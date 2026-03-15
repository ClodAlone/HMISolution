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

#region using Syncfusion

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using System.ComponentModel;
using System.Text;
using System.Collections.Generic;

#endregion

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
    /// The ChartUpdateFlags enumerator.
	/// </summary>
	internal enum ChartUpdateFlags
	{
		/// <summary>
		/// None was changed
		/// </summary>
		None = 0x00,

		/// <summary>
		/// Points was changed
		/// </summary>
		Data = 0x01,

		/// <summary>
		/// Styles was changed
		/// </summary>
		Styles = 0x02,

		/// <summary>
		/// Config items was changed
		/// </summary>
		Config = 0x04,

		/// <summary>
		/// Indexed mode was changed
		/// </summary>
		Indexed = 0x08,

		/// <summary>
		/// Need update regions
		/// </summary>
		Regions = 0x10,

		/// <summary>
		/// Axes was changed 
		/// </summary>
		Ranges = 0x20,

		/// <summary>
		/// All was changed
		/// </summary>
		All = Data | Styles | Indexed | Config | Regions | Ranges
	}

	/// <summary>
	/// Provides the series render arguments.
	/// </summary>
	public abstract class ChartRenderArgs
	{
		#region Members
		private ChartAxis m_xAxis;
		private ChartAxis m_yAxis;

		private DoubleRange m_xRange;
		private DoubleRange m_yRange;

		private bool m_isInvertedAxes;

		private ChartSeries m_series;
		private IChartAreaHost m_chart;
		private int m_seriesIndex;

		private int m_placement;
		private bool m_needUpdateRegions;
		private RectangleF m_bounds;
		private DoubleRange m_sideBySideInfo = DoubleRange.Empty;
		#endregion

		#region Properites
		/// <summary>
		/// Gets or sets the actual X axis.
		/// </summary>
		/// <value>The actual X axis.</value>
		public ChartAxis ActualXAxis
		{
			get { return m_xAxis; }

			set { m_xAxis = value; }
		}

		/// <summary>
		/// Gets or sets the actual Y axis.
		/// </summary>
		/// <value>The actual Y axis.</value>
		public ChartAxis ActualYAxis
		{
			get { return m_yAxis; }

			set { m_yAxis = value; }
		}

		/// <summary>
		/// Gets the visible range of X axis.
		/// </summary>
		/// <value>The X range.</value>
		public DoubleRange XRange
		{
			get { return m_xRange; }
		}

		/// <summary>
		/// Gets the visible range of Y axis.
		/// </summary>
		/// <value>The Y range.</value>
		public DoubleRange YRange
		{
			get { return m_yRange; }
		}

		/// <summary>
		/// Gets the series is being drawn.
		/// </summary>
		/// <value>The series.</value>
		public ChartSeries Series
		{
			get { return m_series; }
		}

		/// <summary>
		/// Gets or sets the chart.
		/// </summary>
		/// <value>The chart.</value>
		public IChartAreaHost Chart
		{
			get { return m_chart; }

			set { m_chart = value; }
		}

		/// <summary>
		/// Gets the index of the series.
		/// </summary>
		/// <value>The index of the series.</value>
		public int SeriesIndex
		{
			get { return m_seriesIndex; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this axes is inverted.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this axes is inverted; otherwise, <c>false</c>.
		/// </value>
		public bool IsInvertedAxes
		{
			get { return m_isInvertedAxes; }

			set { m_isInvertedAxes = value; }
		}

		/// <summary>
		/// Gets a value indicating whether regions should be updated.
		/// </summary>
		/// <value><c>true</c> if regions should be updated; otherwise, <c>false</c>.</value>
		public bool UpdateRegions
		{
			get { return m_needUpdateRegions; }
		}

		/// <summary>
		/// Gets or sets the series position in the depth.
		/// </summary>
		/// <value>The placement.</value>
		public int Placement
		{
			get { return m_placement; }

			set { m_placement = value; }
		}

		/// <summary>
		/// Gets or sets the rectangle that represents the bounds of the series that is being drawn.
		/// </summary>
		/// <value>The bounds.</value>
		public RectangleF Bounds
		{
			get { return m_bounds; }

			set { m_bounds = value; }
		}

		/// <summary>
		/// Gets or sets the side by side info.
		/// </summary>
		/// <value>The side by side info.</value>
		public DoubleRange SideBySideInfo
		{
			get 
			{
				if (m_sideBySideInfo.IsEmpty)
				{
					m_sideBySideInfo = m_series.ChartModel.GetSideBySideInfo(m_chart.GetChartArea(), m_series);
				}

				return m_sideBySideInfo; 
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartRenderArgs"/> class.
		/// </summary>
		/// <param name="chart">The <see cref="IChartAreaHost"/>.</param>
		/// <param name="series">The series.</param>
		public ChartRenderArgs(IChartAreaHost chart, ChartSeries series)
		{
			m_chart = chart;
			m_series = series;

			m_seriesIndex = m_chart.Series.IndexOf(series);
			m_isInvertedAxes = series.RequireInvertedAxes;

			m_needUpdateRegions = chart.NeedRegionUpdate;

			if (m_isInvertedAxes)
			{
				m_yAxis = m_chart.GetChartArea().GetXAxis(series);
				m_xAxis = m_chart.GetChartArea().GetYAxis(series);
			}
			else
			{
				m_xAxis = m_chart.GetChartArea().GetXAxis(series);
				m_yAxis = m_chart.GetChartArea().GetYAxis(series);
			}

			if (m_xAxis.ValueType == ChartValueType.Logarithmic)
			{
				m_xRange = new DoubleRange(Math.Pow(m_xAxis.LogBase, m_xAxis.VisibleRange.min), Math.Pow(m_xAxis.LogBase, m_xAxis.VisibleRange.max));
			}
			else
			{
				m_xRange = new DoubleRange(m_xAxis.VisibleRange.min, m_xAxis.VisibleRange.max);
			}

			if (m_yAxis.ValueType == ChartValueType.Logarithmic)
			{
				m_yRange = new DoubleRange(Math.Pow(m_yAxis.LogBase, m_yAxis.VisibleRange.min), Math.Pow(m_yAxis.LogBase, m_yAxis.VisibleRange.max));
			}
			else
			{
				m_yRange = new DoubleRange(m_yAxis.VisibleRange.min, m_yAxis.VisibleRange.max);
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Determines whether the specified coordinates is visible.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns>
		/// 	<c>true</c> if the specified x is visible; otherwise, <c>false</c>.
		/// </returns>
		public bool IsVisible(double x, double y)
		{
			return m_xRange.Inside(x) && m_yRange.Inside(y);
		}

		/// <summary>
		/// Determines whether the specified ranges is visible.
		/// </summary>
		/// <param name="xRange">The x range.</param>
		/// <param name="yRange">The y range.</param>
		/// <returns>
		/// 	<c>true</c> if the specified x range is visible; otherwise, <c>false</c>.
		/// </returns>
		public bool IsVisible(DoubleRange xRange, DoubleRange yRange)
		{
			return m_xRange.IsIntersects(xRange) && m_yRange.IsIntersects(yRange);
		}

		/// <summary>
		/// Gets the point.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		public virtual PointF GetPoint(double x, double y)
		{
			float xf = m_xAxis.GetCoordinateFromValue(x);
			float yf = m_yAxis.GetCoordinateFromValue(y);

			return m_isInvertedAxes ? new PointF(yf, xf) : new PointF(xf, yf);
		}

		/// <summary>
		/// Gets the rectangle.
		/// </summary>
		/// <param name="x1">The x1.</param>
		/// <param name="y1">The y1.</param>
		/// <param name="x2">The x2.</param>
		/// <param name="y2">The y2.</param>
		/// <returns></returns>
		public RectangleF GetRectangle(double x1, double y1, double x2, double y2)
		{
            x1 = Math.Min(Math.Max(x1, ActualXAxis.Range.Min), ActualXAxis.Range.Max);
            x2 = Math.Min(Math.Max(x2, ActualXAxis.Range.Min), ActualXAxis.Range.Max);
            y1 = Math.Min(Math.Max(y1, ActualYAxis.Range.Min), ActualYAxis.Range.Max);
            y2 = Math.Min(Math.Max(y2, ActualYAxis.Range.Min), ActualYAxis.Range.Max);
			PointF pt1 = this.GetPoint(x1, y1);
			PointF pt2 = this.GetPoint(x2, y2);

			return ChartMath.CorrectRect(pt1.X, pt1.Y, pt2.X, pt2.Y);
		}
		#endregion
	}

	/// <summary>
	/// Provides the series render arguments in 2D mode.
	/// </summary>
	public sealed class ChartRenderArgs2D : ChartRenderArgs
	{
		#region Members
		private ChartGraph m_graph;
		private SizeF m_offset;
		private SizeF m_offsetDepth;
		private bool m_is3D;
        private bool m_chartPerformance;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the <see cref="ChartGraph"/> object.
		/// </summary>
		/// <value>The graph.</value>
		public ChartGraph Graph
		{
			get { return m_graph; }

			set { m_graph = value; }
		}

		/// <summary>
		/// Gets or sets the offset.
		/// </summary>
		/// <value>The offset.</value>
		public SizeF Offset
		{
			get { return m_offset; }

			set { m_offset = value; }
		}

		/// <summary>
		/// Gets or sets the depth offset.
		/// </summary>
		/// <value>The depth offset.</value>
		public SizeF DepthOffset
		{
			get { return m_offsetDepth; }

			set { m_offsetDepth = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether is 3D mode.
		/// </summary>
		/// <value><c>true</c> if is 3D mode; otherwise, <c>false</c>.</value>
		public bool Is3D
		{
			get { return m_is3D; }

			set { m_is3D = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartRenderArgs"/> class.
		/// </summary>
		/// <param name="chart">The <see cref="IChartAreaHost"/>.</param>
		/// <param name="series">The series.</param>
		public ChartRenderArgs2D(IChartAreaHost chart, ChartSeries series)
			: base(chart, series)
		{
			m_is3D = chart.Series3D;
            m_chartPerformance = this.Chart.ImprovePerformance;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Gets the point.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		public override PointF GetPoint(double x, double y)
		{
			PointF ptf = base.GetPoint(x, y);

			if (m_is3D)
			{
				ptf.X += m_offset.Width;
				ptf.Y += m_offset.Height;
			}

            if (!m_chartPerformance)
            {
                RectangleF ClipBounds = this.Chart.GetGraphics().ClipBounds;
                ptf.X = (ptf.X < ClipBounds.X ? ClipBounds.X : ptf.X);
                ptf.Y = (ptf.Y < ClipBounds.Y ? ClipBounds.X : ptf.Y);

                ptf.X = (ptf.X > ClipBounds.Width ? ClipBounds.Width : ptf.X);
                ptf.Y = (ptf.Y > ClipBounds.Width ? ClipBounds.Width : ptf.Y);

            }
           	return ptf;
		}
		#endregion
	}

	/// <summary>
	/// Provides the series render arguments in 3D mode. 
	/// </summary>
	public sealed class ChartRenderArgs3D : ChartRenderArgs
	{
		#region Members
		private Graphics3D m_graph;
		private double m_z;
		private double m_depth;
		#endregion

		#region Proeprties
		/// <summary>
		/// Gets or sets the graph.
		/// </summary>
		/// <value>The graph.</value>
		public Graphics3D Graph
		{
			get { return m_graph; }

			set { m_graph = value; }
		}

		/// <summary>
		/// Gets or sets the Z.
		/// </summary>
		/// <value>The Z.</value>
		public double Z
		{
			get { return m_z; }

			set { m_z = value; }
		}

		/// <summary>
		/// Gets or sets the depth.
		/// </summary>
		/// <value>The depth.</value>
		public double Depth
		{
			get { return m_depth; }

			set { m_depth = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartRenderArgs"/> class.
		/// </summary>
		/// <param name="chart">The <see cref="IChartAreaHost"/>.</param>
		/// <param name="series">The series.</param>
		public ChartRenderArgs3D(IChartAreaHost chart, ChartSeries series)
			: base(chart, series)
		{
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Gets the <see cref="Vector3D"/> by chart values.
		/// </summary>
		/// <param name="x">The X value.</param>
		/// <param name="y">The Y value.</param>
		/// <returns></returns>
		public Vector3D GetVector(double x, double y)
		{
			PointF ptf = this.GetPoint(x, y);

			return new Vector3D(x, y, m_z);
		}
		#endregion
	}

	/// <summary>
	/// <para>
	/// Base class for all renderers. Each renderer is responsible for rendering one data series 
	/// (please refer to <see cref="ChartSeries"/>) inside of the chart area. ChartSeriesRenderer 
	/// provides the basic plumbing that is needed by all renderers. It is not an abstract class. 
	/// It is used as the renderer for the scatter plot since the scatter plot needs only basic 
	/// point rendering at the correct position.
	/// </para>
	/// <para>
	/// You can derive from ChartSeriesRenderer to create your own renderers.
	/// </para>
	/// </summary>
	public class ChartSeriesRenderer
	{
		#region Helper classes
		/// <summary>
		/// This class is using for the caching points and styles.
		/// </summary>
		protected class ChartStyledPoint : ChartPointWithIndex, ICloneable
		{
			#region Members
			private double m_x;
			private double[] m_yValues;
			private bool m_isVisible;

			private ChartStyleInfo m_style;
			private string m_toolTip;
			#endregion

			#region Properties
			/// <summary>
			/// Gets or sets the X.
			/// </summary>
			/// <value>The X.</value>
			/// <remarks>
			/// In indexed mode it's the index of real X value.
			/// </remarks>
			public double X
			{
				get { return m_x; }
				set { m_x = value; }
			}
			/// <summary>
			/// Gets or sets the Y values.
			/// </summary>
			/// <value>The Y values.</value>
			public double[] YValues
			{
				get { return m_yValues; }
				set { m_yValues = value; }
			}
			/// <summary>
			/// Gets or sets a value indicating whether this point is visible.
			/// </summary>
			/// <value>
			/// 	<c>true</c> if this point is visible; otherwise, <c>false</c>.
			/// </value>
			public bool IsVisible
			{
				get { return m_isVisible; }
				set { m_isVisible = value; }
			}
			/// <summary>
			/// Gets or sets the specified style of point.
			/// </summary>
			public ChartStyleInfo Style
			{
				get { return m_style; }
				set { m_style = value; }
			}
			/// <summary>
			/// Gets or sets the tooltip of point.
			/// </summary>
			public string ToolTip
			{
				get { return m_toolTip; }
				set { m_toolTip = value; }
			}
			#endregion

			#region Constructor
			/// <summary>
			/// Initialize the new instance.
			/// </summary>
			/// <param name="cp"></param>
			/// <param name="index"></param>
			public ChartStyledPoint(ChartPoint cp, int index)
				: base(cp, index)
			{
			}
			#endregion

			#region Public methods
			/// <summary>
			/// Creates a new object that is a copy of the current instance.
			/// </summary>
			/// <returns>
			/// A new object that is a copy of this instance.
			/// </returns>
			public ChartStyledPoint Clone()
			{
				ChartStyledPoint duplicate = new ChartStyledPoint(this.Point, this.Index);

				duplicate.m_style = m_style;
				duplicate.m_toolTip = m_toolTip;

				return duplicate;
			}
			/// <summary>
			/// Creates a new object that is a copy of the current instance.
			/// </summary>
			/// <returns>
			/// A new object that is a copy of this instance.
			/// </returns>
			object ICloneable.Clone()
			{
				return this.Clone();
			}
			#endregion
		}
		/// <summary>
		/// This class is using for sorting <see cref="ChartStyledPoint"/> by X or Index values.
		/// </summary>
		protected class ChartStyledPointComparer : IComparer
		{
			#region Implementation
			/// <summary>
			/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
			/// </summary>
			/// <param name="x">The first object to compare.</param>
			/// <param name="y">The second object to compare.</param>
			/// <returns>
			/// Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y.
			/// </returns>
			/// <exception cref="T:System.ArgumentException">Neither x nor y implements the <see cref="T:System.IComparable"></see> interface.-or- x and y are of different types and neither one can handle comparisons with the other. </exception>
			int System.Collections.IComparer.Compare(object x, object y)
			{
				ChartStyledPoint p1 = (x as ChartStyledPoint);
				ChartStyledPoint p2 = (y as ChartStyledPoint);

				if (p1.X < p2.X)
				{
					return -1;
				}

				if (p1.X > p2.X)
				{
					return 1;
				}

				if (p1.Index < p2.Index) return -1;
				if (p1.Index > p2.Index) return 1;

				return 0;
			}
			#endregion
		}
		#endregion

		#region Constants
		/// <summary>
		/// The number of polygons of cylinder
		/// </summary>
		protected int POLYGON_SECTORS = 16;
		/// <summary>
		/// The number of polygons of spline
		/// </summary>
		protected int SPLINE_DIGITIZATION = 10;
		#endregion

		#region Members
		/// <summary>
		/// The owner series.
		/// </summary>
		[DocumentationExclude()]
		protected ChartSeries m_series;
		private RectangleF m_bounds;
		private IChartAreaHost m_chart = null;

		private ChartAxis m_xAxis;
		private ChartAxis m_yAxis;
        private bool m_chartPerformance;
		/// <summary>
		/// Internal member.
		/// </summary>
		protected ChartSegment[] m_segments;

		private ArrayList m_styles = new ArrayList();
		private bool m_shouldUpdate = true;
		/// <summary>
		/// The series style.
		/// </summary>
		protected ChartStyleInfo m_serStyle = null;
		private int m_place = 0;
		private int m_placeSize = 0;
		private bool m_styleUpdating = false;

		private ChartLabelLayoutManager m_labelLayoutManager = null;

		private IndexRange[] m_unEmptyRanges = null;

		private ChartStyledPoint[] m_points = null;
		private bool m_isSorted = false;

		private List<ChartStyledPoint> m_pointsCache = null;
        private List<int> m_pointIndex=new List<int>();
        private ChartStyledPoint[] m_styledPoint;
		#endregion

       
		#region Properties
        /// <summary>
        ///  Retruns all Styled Point Collection.
        /// </summary>
        protected ChartStyledPoint[] StyledPoints
        {
            get
            {
                return m_styledPoint;
            }
            set
            {
                m_styledPoint = value;
            }
        }
		/// <summary>
		/// Number of layer for specified series.
		/// </summary>
		public int Place
		{
			get
			{
				return m_place;
			}
			set
			{
				m_place = value;
			}
		}

		/// <summary>
		/// Count of the chart layers.
		/// </summary>
		public int PlaceSize
		{
			get
			{
				return m_placeSize;
			}
			set
			{
				m_placeSize = value;
			}
		}

		/// <summary>
		/// Indicates how much space this type will use.
		/// </summary>
		public virtual ChartUsedSpaceType FillSpaceType
		{
			get
			{
				return ChartUsedSpaceType.OneForOne;
			}
		}

		/// <summary>
		/// Gets the center of <see cref="ChartSeriesRenderer.Bounds"/>.
		/// </summary>
		/// <value>The center.</value>
		protected PointF Center
		{
			get
			{
				return new PointF(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);
                
			}
		}

		/// <summary>
		/// Gets the series style.
		/// </summary>
		/// <value>The series style.</value>
		internal ChartStyleInfo SeriesStyle
		{
			get
			{
				if (m_serStyle == null)
				{
					m_serStyle = m_series.GetOfflineStyle();
				}

				return m_serStyle;
			}
		}

		/// <summary>
		/// Gets array of geometry primitives for sorting before visualizting.
		/// </summary>
		internal ChartSegment[] Segments
		{
			get
			{
				return m_segments;
			}
		}

		/// <summary>
		/// Computes the array of <see cref="IndexRange"/>, using for indicating unempty points.
		/// </summary>
		protected IndexRange[] UnEmptyRanges
		{
			get
			{
				if (m_unEmptyRanges == null)
				{
					m_unEmptyRanges = this.CalculateUnEmptyRanges(new IndexRange(0, m_series.Points.Count - 1));
				}

				return m_unEmptyRanges;
			}
		}

		/// <summary>
		/// Duplicates the <see cref="ChartSeries.EnableStyles"/> property.
		/// </summary>
		protected bool EnableStyles
		{
			get
			{
				return m_series.EnableStyles;
			}
		}

		/// <summary>
		/// Returns the bounds that this renderer operates in.
		/// </summary>
		protected RectangleF Bounds
		{
			get
			{
				return m_bounds;
			}
		}

		/// <summary>
		/// Reference to the <see cref="IChartAreaHost"/> instance that uses this instance.
		/// </summary>
		protected IChartAreaHost Chart
		{
			get
			{
				return m_chart;
			}
		}

		/// <summary>
		/// Reference to the <see cref="ChartArea"/> instance that uses this instance.
		/// </summary>
		protected IChartArea ChartArea
		{
			get
			{
				return m_chart.GetChartArea();
			}
		}

		/// <summary>
		/// Returns the X coordinate of the origin. This property will return the correct coordinate even if the X axis has a custom
		/// origin.
		/// </summary>
		protected virtual float CustomOriginX
		{
			get
			{
				float f = 0;
				ChartAxis xAxis = XAxis;

				if (xAxis.CustomOrigin)
				{
					f = this.GetXFromCoordinate(xAxis.Origin);
				}
				else
				{
					f = this.GetXFromCoordinate(0d);
				}

				return f;
			}
		}

		/// <summary>
		/// Returns the Y coordinate of the origin. This property will return the correct coordinate even if the Y axis has a custom
		/// origin.
		/// </summary>
		protected virtual float CustomOriginY
		{
			get
			{
				float f = 0;
				ChartAxis yAxis = YAxis;

				if (yAxis.CustomOrigin)
				{
					f = this.GetYFromCoordinate(yAxis.Origin);
				}
				else
				{
					f = this.GetYFromCoordinate(0d);
				}

				return f;
			}
		}

		/// <summary>
		/// This setting allows chart types that are normally not rendered inverted to be combined with those that are
		/// normally rendered inverted. For example Bar charts are rendered inverted. The Bubble chart can be combined with
		/// Bar charts because it sets IgnoreSeriesInversion to true. When this property is set to true the renderer will ignore
		/// the inversion setting on the series being rendered.
		/// </summary>
		protected virtual bool IgnoreSeriesInversion
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// The location of the origin as used for rendering.
		/// </summary>
		protected virtual PointF OriginLocation
		{
			get
			{
				return new PointF(this.GetXFromCoordinate(0d), this.GetYFromCoordinate(0d));
			}
		}

		/// <summary>
		/// Returns the X axis object that the current renderer is tied to.
		/// <seealso cref="ChartAxis"/>
		/// </summary>
		protected ChartAxis XAxis
		{
			get
			{
				if (m_xAxis == null)
				{
					m_xAxis = m_series.XAxis;
				}

				return m_xAxis;
			}
		}

		/// <summary>
		/// Returns the Y axis object that the current renderer is tied to.
		/// <seealso cref="ChartAxis"/>
		/// </summary>
		protected ChartAxis YAxis
		{
			get
			{
				if (m_yAxis == null)
				{
					m_yAxis = m_series.YAxis;
				}

				return m_yAxis;
			}
		}

		/// <summary>
		/// Get description of regions.
		/// </summary>
		protected virtual string RegionDescription
		{
			get
			{
				return "Series Chart Region";
			}
		}

		/// <summary>
		/// Gets count of require Y values of the points.
		/// </summary>
		protected virtual int RequireYValuesCount
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Gets a value indicating whether points should be sort.
		/// </summary>
		/// <value><c>true</c> if points should be sorted; otherwise, <c>false</c>.</value>
		protected virtual bool ShouldSort
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// True if axes is inverted.
		/// </summary>
		protected virtual bool IsInvertedAxes
		{
			get
			{
				return m_series.RequireInvertedAxes || (this.IgnoreSeriesInversion && m_chart.RequireInvertedAxes);
			}
		}

		/// <summary>
		/// True if series using the radial axes.
		/// </summary>
		protected virtual bool IsRadial
		{
			get
			{
				return m_series.Type == ChartSeriesType.Radar || m_series.Type == ChartSeriesType.Polar;
			}
		}

		/// <summary>
		/// Gets the minimal points delta.
		/// </summary>
		/// <returns></returns>
		internal double GetMinPointsDelta()
		{
			double minPointsDelta = double.MaxValue;

			foreach (ChartSeries series in m_chart.Series)
			{
				if (series.Visible)
				{
					double[] xValues = new double[series.Points.Count];

					for (int i = 0; i < series.Points.Count; i++)
					{
						xValues[i] = series.Points[i].X;
					}

					Array.Sort(xValues);

					for (int i = 1; i < xValues.Length; i++)
					{
						double delta = xValues[i] - xValues[i - 1];

						if (delta != 0)
						{
							minPointsDelta = Math.Min(minPointsDelta, delta);
						}
					}
				}
			}

			return minPointsDelta == double.MaxValue ? 1d : minPointsDelta;
		}

		/// <summary>
		/// Computes and returns the space occupied by each interval on the series being rendered.
		/// </summary>
		public virtual SizeF IntervalSpace
		{
			get
			{
				ChartAxis xAxis = XAxis;
				ChartAxis yAxis = YAxis;

				double width = xAxis.VisibleRange.Interval * m_bounds.Width / xAxis.VisibleRange.Delta;
				double height = yAxis.VisibleRange.Interval * m_bounds.Height / yAxis.VisibleRange.Delta;

				//if (xAxis.Inversed)
				//{
				//  width = -width;
				//}

				//if (yAxis.Inversed)
				//{
				//  height = -height;
				//}

				return new SizeF((float)width, (float)height);
			}
		}

		/// <summary>
		/// Calculates and returns the number of display units that are used per logical(value) unit.
		/// </summary>
		public SizeF DividedIntervalSpace
		{
			get
			{
				SizeF space = IntervalSpace;

				space.Width /= (float)XAxis.VisibleRange.Interval;
				space.Height /= (float)YAxis.VisibleRange.Interval;

				return space;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="series" type="Syncfusion.Windows.Forms.Chart.ChartSeries">
		///     <para>
		///     ChartSeries that will be rendered by this renderer instance.
		///     </para>
		/// </param>
		public ChartSeriesRenderer(ChartSeries series)
		{
			if (series == null)
				throw new ArgumentNullException("series");

			m_series = series;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Renders chart by the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public virtual void Render(ChartRenderArgs2D args)
		{
		}

		/// <summary>
		/// Renders chart by the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		public virtual void Render(ChartRenderArgs3D args)
		{
		}

		/// <summary>
		///     In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does 
		///     the rendering. 
		/// </summary>
		/// <param name="g" type="System.Drawing.Graphics">
		///     <para>
		///     The graphics object that is to be used for rendering.
		///     </para>
		/// </param>
		public virtual void Render(Graphics g)
		{
			ChartRenderArgs2D renderArgs = new ChartRenderArgs2D(m_chart, m_series);

			renderArgs.Graph = new ChartGDIGraph(g);
			renderArgs.Bounds = m_bounds;

			if (renderArgs.Is3D)
			{
				renderArgs.Offset = this.GetThisOffset();
				renderArgs.DepthOffset = this.GetSeriesOffset();
			}

			this.Render(renderArgs);
		}

		/// <summary>
		///     Renders series name in the minimal position of all axes.
		/// </summary>
		/// <param name="g" type="System.Drawing.Graphics">
		///     <para>
		///     The graphics object that is to be used for rendering.
		///     </para>
		/// </param>
		public virtual void RenderSeriesNameInDepth(Graphics g)
		{
			if (!m_series.DrawSeriesNameInDepth || !m_chart.Series3D) return;
			SizeF offset = GetSeriesOffset();
			SizeF serOffset = GetThisOffset();
			ChartPoint cp = new ChartPoint(m_series.ActualXAxis.VisibleRange.Min, m_series.ActualYAxis.VisibleRange.Min);
			PointF point = new PointF(GetXFromValue(cp, 0) + serOffset.Width + offset.Width / 2, GetYFromValue(cp, 0) + serOffset.Height + offset.Height / 2);
			ChartStyleInfo style = SeriesStyle;

			Size sz = g.MeasureString(m_series.Name, SeriesStyle.Font.GdipFont).ToSize();
			GraphicsPath gp = new GraphicsPath();
			gp.AddString(m_series.Name, SeriesStyle.Font.GdipFont.FontFamily, (int)SeriesStyle.Font.GdipFont.Style, RenderingHelper.GetFontSizeInPixels(SeriesStyle.Font.GdipFont),
				new PointF(point.X, point.Y - sz.Height / 2), Graphics3D.DefaultStrinfFormat);


			if (gp.PointCount > 0)
			{
				g.FillPath(new SolidBrush(SeriesStyle.TextColor), gp);
			}
		}

		/// <summary>
		///     In the base <see cref="ChartSeriesRenderer"/> it does not do anything. In derived classes this function does 
		///     the rendering. 
		/// </summary>
		/// <param name="g" type="System.Drawing.Graphics">
		///     <para>
		///     The graphics object that is to be used for rendering.
		///     </para>
		/// </param>
		public virtual void Render(Graphics3D g)
		{
			ChartRenderArgs3D renderArgs = new ChartRenderArgs3D(m_chart, m_series);

			renderArgs.Graph = g;
			renderArgs.Z = this.GetPlaceDepth();
			renderArgs.Depth = this.GetSeriesDepth();
            renderArgs.Bounds = m_bounds;            
			this.Render(renderArgs);
		}

		/// <summary>
		/// Renders series name in the minimal position of all axes.
		/// </summary>
		/// <param name="g" type="System.Drawing.Graphics">
		///     <para>
		///     The graphics object that is to be used for rendering.
		///     </para>
		/// </param>
		public virtual void RenderSeriesNameInDepth(Graphics3D g)
		{
			if (!m_series.DrawSeriesNameInDepth || !m_chart.Series3D) return;
			float fd = GetPlaceDepth();
			float dpth = GetSeriesDepth();
			float middleD = fd + dpth / 2;
			ChartPoint cp = new ChartPoint(m_series.ActualXAxis.VisibleRange.Min, m_series.ActualYAxis.VisibleRange.Min);
			ChartPoint cp2 = new ChartPoint(m_series.ActualXAxis.VisibleRange.Max, m_series.ActualYAxis.VisibleRange.Max);
			PointF point = new PointF(GetXFromValue(cp, 0), GetYFromValue(cp, 0));
			PointF point2 = new PointF(GetXFromValue(cp2, 0), GetYFromValue(cp2, 0));
			ChartStyleInfo style = SeriesStyle;


			Size sz = g.Graphics.MeasureString(m_series.Name, SeriesStyle.Font.GdipFont).ToSize();
			GraphicsPath gp = new GraphicsPath();
			gp.AddString(m_series.Name, SeriesStyle.Font.GdipFont.FontFamily, (int)SeriesStyle.Font.GdipFont.Style, RenderingHelper.GetFontSizeInPixels(SeriesStyle.Font.GdipFont),
				new PointF(0, -sz.Height / 2), Graphics3D.DefaultStrinfFormat);


			if (gp.PointCount > 0)
			{
				g.AddPolygon(new Polygon(new Vector3D[]{
                                                   new Vector3D( point.X, point.Y, fd ),
                                                   new Vector3D( point.X, point.Y, fd + dpth ),
                                                   new Vector3D( point2.X, point2.Y, fd + dpth ),
                                                   new Vector3D( point2.X, point2.Y, fd ),
                                                 }));
				Path3D p3d = Path3D.FromGraphicsPath(gp, 0, new SolidBrush(SeriesStyle.TextColor));
				p3d.Transform(Matrix3D.RotateAlongOX(m_series.SeriesNameOXAngle * (float)Math.PI / 180f));
				p3d.Transform(Matrix3D.Transform(point.X, point.Y, middleD));
				g.AddPolygon(p3d);
			}
		}

		/// <summary>
		/// Draws the icon on the legend.
		/// </summary>
		/// <param name="g">Instance of <see cref="Graphics"/>.</param>
		/// <param name="bounds">Bounds of icon.</param>
		/// <param name="isShadow">If is true method draws the shadow.</param>
		/// <param name="shadowColor"><see cref="Color"/> of shadow.</param>
		public virtual void DrawIcon(Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
		{
			if (isShadow)
			{
				using (SolidBrush br = new SolidBrush(shadowColor))
				{
					g.FillRectangle(br, bounds);
				}
			}
			else
			{
				BrushPaint.FillRectangle(g, bounds, this.SeriesStyle.Interior);
				g.DrawRectangle(SeriesStyle.GdipPen, bounds);
			}
		}

		/// <summary>
		/// Draws the icon on the legend.
		/// </summary>
		/// <param name="index">Index of point.</param>
		/// <param name="g">Instance of <see cref="Graphics"/>.</param>
		/// <param name="bounds">Bounds of icon.</param>
		/// <param name="isShadow">If is true method draws the shadow.</param>
		/// <param name="shadowColor"><see cref="Color"/> of shadow.</param>
		public virtual void DrawIcon(int index, Graphics g, Rectangle bounds, bool isShadow, Color shadowColor)
		{
			if (isShadow)
			{
				using (SolidBrush br = new SolidBrush(shadowColor))
				{
					g.FillRectangle(br, bounds);
				}
			}
			else
			{
				ChartStyleInfo style = this.GetStyleAt(index);
				BrushPaint.FillRectangle(g, bounds, style.Interior);
				g.DrawRectangle(style.GdipPen, bounds);
			}
		}

		/// <summary>
		/// Checks the count of values for rendering.
		/// </summary>
		/// <returns>True if renderer can to render the series.</returns>
		public virtual bool CanRender()
		{
			this.StyledPoints = this.PrepearePoints();

            foreach (ChartStyledPoint point in this.StyledPoints)
			{
				if (point.IsVisible)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Computes the size of necessary rectangle for the rendering.
		/// </summary>
		/// <returns><see cref="SizeF"/> of minimal rectangle.</returns>
		public virtual SizeF GetMinSize(Graphics g)
		{
			return (Chart != null) ? ChartArea.MinSize : new SizeF(200, 100);
		}

		/// <summary>
		/// Sets the chart to representation.
		/// </summary>
		/// <param name="chart">The chart.</param>
		public void SetChart(IChartAreaHost chart)
		{
			m_chart = chart;
			m_xAxis = chart.GetChartArea().GetXAxis(m_series);
			m_yAxis = chart.GetChartArea().GetYAxis(m_series);
		}

		/// <summary>
		/// Gets character point by index. Used for symbols and fancy tooltips.
		/// </summary>
		/// <param name="index">Index of point.</param>
		/// <returns></returns>
		public virtual PointF GetCharacterPoint(int index)
		{
			PointF result = PointF.Empty;

			if (this.ChartArea.Series3D && this.ChartArea.RealSeries3D)
			{
				Vector3D v3d = this.GetSymbolVector(this.GetStyledPoint(index));
				result = this.ChartArea.Transform3D.ToScreen(v3d);
			}
			else
			{
				result = this.GetSymbolPoint(this.GetStyledPoint(index));
			}

			return result;
		}

		/// <summary>
		/// Updates by specified flags.
		/// </summary>
		/// <param name="flags">The flags.</param>
		internal virtual void Update(ChartUpdateFlags flags)
		{
			if (!m_styleUpdating)
			{
				if ((flags & ChartUpdateFlags.Data) != ChartUpdateFlags.None)
				{
					m_points = null;
					m_unEmptyRanges = null;
				}

				if ((flags & ChartUpdateFlags.Styles) != ChartUpdateFlags.None)
				{
					m_shouldUpdate = true;
					m_serStyle = null;
				}

				if ((flags & ChartUpdateFlags.Indexed) != ChartUpdateFlags.None)
				{
					if (m_points != null)
					{
						foreach (ChartStyledPoint styledPoint in m_points)
						{
							styledPoint.X = this.GetIndexValueFromX(styledPoint.Point.X);
						}
					}
				}
			}
		}

		/// <summary>
		/// Updates the points cache.
		/// </summary>
		/// <param name="args">The <see cref="System.ComponentModel.ListChangedEventArgs"/> instance containing the event data.</param>
		internal virtual void DataUpdate(ListChangedEventArgs args)
		{
			m_points = null;
			m_unEmptyRanges = null;

			switch (args.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					this.InsertPoint(args.NewIndex);
					break;
				case ListChangedType.ItemChanged:
					this.UpdatePoint(args.NewIndex);
					break;
				case ListChangedType.ItemDeleted:
					this.RemovePoint(args.NewIndex);
					break;
				case ListChangedType.Reset:
					this.ResetCache();
					break;
			}
		}
		#endregion

		#region Implementation

		#region Obsolete methods
        /// <summary>
        /// Gets the point by value for series.
        /// </summary>
        /// <param name="chpt">The ChartPoint.</param>
        /// <returns> Returns Real Point for the Specified ChartPoint </returns>
		[Obsolete("This method isn't used anymore. Use GetCharacterPoint method.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual PointF GetPointByValueForSeries(ChartPoint chpt)
		{
			PointF result = this.GetPointFromValue(chpt);

			if (ChartArea.Series3D && ChartArea.RealSeries3D)
			{
				double depth = this.GetPlaceDepth();
				result = ChartArea.Transform3D.ToScreen(new Vector3D(result.X, result.Y, depth));
			}
			else
			{
				SizeF offset = this.GetThisOffset();
				result = new PointF(result.X + offset.Width, result.Y + offset.Height);
			}

			return result;
		}
		#endregion

		#region Computes offsets and depths of the specified series.
		/// <summary>
		/// Clones the points and shifts by offset.
		/// </summary>
		/// <param name="points"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		protected PointF[] GetOffsetPoints(PointF[] points, SizeF offset)
		{
			PointF[] offsetPoints = new PointF[points.Length];

			for (int i = 0; i < points.Length; i++)
			{
				offsetPoints[i] = new PointF(points[i].X + offset.Width, points[i].Y + offset.Height);
			}

			return offsetPoints;
		}

		/// <summary>
		/// Calculates depth offset.
		/// </summary>
		/// <returns>Series offset.</returns>
		protected virtual SizeF GetSeriesOffset()
		{
			if (Chart.Series3D)
			{
				float depth = (1f - 0.01f * m_chart.SpacingBetweenSeries) / m_placeSize;

				return new SizeF(depth * this.ChartArea.OffsetX, -depth * this.ChartArea.OffsetY);
			}

			return SizeF.Empty;
		}

		/// <summary>
		/// Gets the depth size of series.
		/// </summary>
		/// <returns></returns>
		protected virtual float GetSeriesDepth()
		{
			if (Chart.Series3D)
			{
				float t = (100f - 2 * m_chart.SpacingBetweenSeries) / 100f;
				return t * this.ChartArea.Depth / PlaceSize;
			}

			return 0;
		}

		/// <summary>
		/// Gets the this series offset.
		/// </summary>
		/// <returns></returns>
		protected virtual SizeF GetThisOffset()
		{
			if (Chart.Series3D)
			{
				float place = (m_place + 0.005f * m_chart.SpacingBetweenSeries) / m_placeSize;
				return new SizeF(place * ChartArea.OffsetX, -place * ChartArea.OffsetY);
			}

			return SizeF.Empty;
		}

		/// <summary>
		/// Gets the depth offset of series.
		/// </summary>
		/// <returns></returns>
		protected virtual float GetPlaceDepth()
		{
			float depth = 0;

			if (Chart.Series3D)
			{
				float t = (m_place + m_chart.SpacingBetweenSeries / (2 * 100));
				depth = t * ChartArea.Depth / m_placeSize;
			}

			return depth;
		}

		/// <summary>
		/// Overloaded. Calculates step point's offsets in derived classes to draw correctly series with close or same values.
		/// This method is needed only in cases when series are rendered in 3D mode.
		/// It fixes problems with overlapped series.
		/// </summary>
		/// <param name="stepPoints"></param> 
		protected virtual void CalculateStepPointsForSeries3D(ref PointF[] stepPoints)
		{
			if (Chart.Series3D)
			{
				CalculateStepPointsForSeries3D(ref stepPoints, GetThisOffset());
			}
		}

		/// <summary>
		/// Calculates step point's offsets in derived classes to draw correctly series with close or same values.
		/// This method is needed only in cases when series are rendered in 3D mode.
		/// It fixes problems with overlapped series.
		/// </summary>
		/// <param name="stepPoints"></param>
		/// <param name="offset"></param>
		protected virtual void CalculateStepPointsForSeries3D(ref PointF[] stepPoints, SizeF offset)
		{
			for (int i = 0; i < stepPoints.Length; i++)
			{
				stepPoints[i].X += offset.Width;
				stepPoints[i].Y += offset.Height;
			}
		}
		#endregion

		#region Get Financial Interiors
		/// <summary>
		/// Returns the up interior for financial chart types.
		/// </summary>
		/// <param name="original">The base interior of chart.</param>
		/// <returns>The <see cref="BrushInfo"/> for the up price sectors.</returns>
		protected virtual BrushInfo GetUpPriceInterior(BrushInfo original)
		{
			BrushInfo result = null;

			switch (m_series.ConfigItems.FinancialItem.ColorsMode)
			{
				case ChartFinancialColorMode.Fixed:
					result = new BrushInfo(m_series.ConfigItems.FinancialItem.PriceUpColor);
					break;

				case ChartFinancialColorMode.Mixed:
					result = DrawingHelper.AddColor(original, m_series.ConfigItems.FinancialItem.PriceUpColor);
					break;

				case ChartFinancialColorMode.DarkLight:
					result = DrawingHelper.AddColor(original, m_series.ConfigItems.FinancialItem.DarkLightPower);
					break;
			}

			return result;
		}

		/// <summary>
		/// Returns the down interior for financial chart types.
		/// </summary>
		/// <param name="original">The base interior of chart.</param>
		/// <returns>The <see cref="BrushInfo"/> for the down price sectors.</returns>
		protected virtual BrushInfo GetDownPriceInterior(BrushInfo original)
		{
			BrushInfo result = null;

			switch (m_series.ConfigItems.FinancialItem.ColorsMode)
			{
				case ChartFinancialColorMode.Fixed:
					result = new BrushInfo(m_series.ConfigItems.FinancialItem.PriceDownColor);
					break;

				case ChartFinancialColorMode.Mixed:
					result = DrawingHelper.AddColor(original, m_series.ConfigItems.FinancialItem.PriceDownColor);
					break;

				case ChartFinancialColorMode.DarkLight:
					result = DrawingHelper.AddColor(original, -m_series.ConfigItems.FinancialItem.DarkLightPower);
					break;
			}

			return result;
		}
		#endregion

		#region Draw 3D Spline
		/// <summary>
		/// Draws 3D Spline from points array and additionally second derivatives added in y2 array.
		/// Remember that second derivatives should be calculated in naturalSpline function and
		/// improper y2[] values can cause improper spline drawing. 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="points"></param>
		/// <param name="y2"></param>
		/// <param name="offset"></param>
		/// <param name="brush"></param>
		/// <param name="pen"></param>
		protected virtual Region Draw3DSpline(Graphics g, ChartPointWithIndex[] points, double[] y2, SizeF offset, BrushInfo brush, Pen pen)
		{
			SizeF serOffset = GetThisOffset();

			ChartPointWithIndex[] pointsExtr;
			double[] y2Extr;
			ChartPointWithIndex p0, p1, p2, p3;
			PointF pf0, pf1, pf2, pf3;
			Region rgn = new Region(new Rectangle(0, 0, 0, 0));
			Pen fillPen = new Pen(brush.BackColor, pen.Width);
			if (points.Length != y2.Length)
				return rgn;

			//Adds minimum and maximum points to existing points.
			AddExtremumPoints(points, y2, out pointsExtr, out y2Extr);

			GraphicsPath top = new GraphicsPath();
			int j;
			int i = j = 0, di = 1, mi = pointsExtr.Length - 1;
			if (XAxis.Inversed)
			{
				i = pointsExtr.Length - 1;
				j = points.Length - 1;
				di = -1;
				mi = 0;
			}

			for (; i != mi; i += di)
			{
				BezierPointsFromSpline(pointsExtr[i], pointsExtr[i + di], y2Extr[i], y2Extr[i + di], out p0, out p1, out p2, out p3);

				pf0 = new PointF(GetXFromValue(p0.Point, 0) + serOffset.Width, GetYFromValue(p0.Point, 0) + serOffset.Height);
				pf1 = new PointF(GetXFromValue(p1.Point, 0) + serOffset.Width, GetYFromValue(p1.Point, 0) + serOffset.Height);
				pf2 = new PointF(GetXFromValue(p2.Point, 0) + serOffset.Width, GetYFromValue(p2.Point, 0) + serOffset.Height);
				pf3 = new PointF(GetXFromValue(p3.Point, 0) + serOffset.Width, GetYFromValue(p3.Point, 0) + serOffset.Height);

				top.AddBezier(pf0, pf1, pf2, pf3);
				top.AddLine(pf3.X, pf3.Y, pf3.X + offset.Width, pf3.Y + offset.Height);
				top.AddBezier(pf3.X + offset.Width, pf3.Y + offset.Height, pf2.X + offset.Width, pf2.Y + offset.Height, pf1.X + offset.Width, pf1.Y + offset.Height, pf0.X + offset.Width, pf0.Y + offset.Height);
				top.CloseAllFigures();
                ChartRenderArgs2D args = new ChartRenderArgs2D(Chart, m_series);
                args.Graph = new ChartGDIGraph(g);
                if (!Chart.Style3D)
                {
                    BrushPaint.FillPath(g, top, brush);
                }
                else
                    this.Draw(args.Graph, top, brush, pen);
				rgn.Union(top);
				top.Reset();

				//Drawing line frame 
				g.DrawBezier(pen, pf0, pf1, pf2, pf3);
				g.DrawBezier(pen, pf3.X + offset.Width, pf3.Y + offset.Height, pf2.X + offset.Width, pf2.Y + offset.Height, pf1.X + offset.Width, pf1.Y + offset.Height, pf0.X + offset.Width, pf0.Y + offset.Height);

				if (pointsExtr[i].Point.X == points[j].Point.X)
				{
					pf0 = new PointF(GetXFromValue(points[j].Point, 0) + serOffset.Width, GetYFromValue(points[j].Point, 0) + serOffset.Height);
					pf3 = new PointF(GetXFromValue(points[j + di].Point, 0) + serOffset.Width, GetYFromValue(points[j + di].Point, 0) + serOffset.Height);

					g.DrawLine(pen, pf0.X, pf0.Y, pf0.X + offset.Width, pf0.Y + offset.Height);
					g.DrawLine(pen, pf3.X, pf3.Y, pf3.X + offset.Width, pf3.Y + offset.Height);
					j += di;
				}
				else
				{
					fillPen.Color = GetBrush(j).BackColor;
                    if (!Chart.Style3D)
					g.DrawLine(fillPen, pf0.X, pf0.Y, pf0.X + offset.Width, pf0.Y + offset.Height);
				}
			}
			return rgn;
		}

		/// <summary>
		///  Adds all extremum points to new arrays. This method is needed to imitate 3D Spline strip.
		/// </summary>
		/// <param name="points"></param>
		/// <param name="y2"></param>
		/// <param name="pointsNew"></param>
		/// <param name="y2New"></param>
		protected void AddExtremumPoints(ChartPointWithIndex[] points, double[] y2, out ChartPointWithIndex[] pointsNew, out double[] y2New)
		{
			int n = points.Length;

			ArrayList pointList = new ArrayList(n * 2);
			ArrayList y2List = new ArrayList(n * 2);
			double a, b, c, deltaX2, t1, t2, temp = 0;

			for (int i = 0; i <= n - 2; i++)
			{
				pointList.Add(points[i]);
				y2List.Add(y2[i]);

				deltaX2 = (points[i + 1].Point.X - points[i].Point.X);
				deltaX2 *= deltaX2;

				a = 0.5d * deltaX2 * (y2[i + 1] - y2[i]);
				b = deltaX2 * y2[i];
				c = (points[i + 1].Point.YValues[0] - points[i].Point.YValues[0]) - deltaX2 * (1 / (3.0d) * y2[i] + 1 / 6.0d * y2[i + 1]);

				if (ChartMath.SolveQuadraticEquation(a, b, c, out t1, out t2))
				{
					if ((t1 > 0) && (t1 < 1))
					{
						temp = points[i].Point.YValues[0] + t1 * (c + t1 * (0.5d * b + t1 * 1 / (3.0d) * a));
						pointList.Add(new ChartPointWithIndex(new ChartPoint(points[i].Point.X + t1 * (points[i + 1].Point.X - points[i].Point.X), temp), points[i].Index));
						y2List.Add(y2[i] + t1 * (y2[i + 1] - y2[i]));
					}
					if ((t2 > 0) && (t2 < 1))
					{
						temp = points[i].Point.YValues[0] + t2 * (c + t2 * (0.5d * b + t2 * 1 / (3.0d) * a));
						pointList.Add(new ChartPointWithIndex(new ChartPoint(points[i].Point.X + t2 * (points[i + 1].Point.X - points[i].Point.X), temp), points[i + 1].Index));
						y2List.Add(y2[i] + t2 * (y2[i + 1] - y2[i]));
					}
				}
			}

			pointList.Add(points[n - 1]);
			y2List.Add(y2[n - 1]);

			//making output
			int m = y2List.Count;
			y2New = new double[m];

			for (int i = 0; i < m; i++)
			{
				y2New[i] = (double)y2List[i];
			}

			m = pointList.Count;
			pointsNew = new ChartPointWithIndex[m];
			for (int i = 0; i < m; i++)
			{
				pointsNew[i] = (ChartPointWithIndex)pointList[i];
			}
		}

		/// <summary>
		/// Given the array of chart points. The procedure returns array of second derivatives of cubic splines at this points.
		/// Then we can get bezier curve coordinates from the second derivatives and points array.
		/// </summary>
		/// <param name="points">The points.</param>
		/// <param name="ys2">The ys2.</param>
		protected void NaturalSpline(ChartPointWithIndex[] points, out double[] ys2)
		{
			int count = points.Length;
			int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];

			ys2 = new double[count];

			double a = 6;
			double[] u = new double[count - 1];
			double p;

			ys2[0] = u[0] = 0;
			ys2[count - 1] = 0;

			for (int i = 1; i < count - 1; i++)
			{
				double d1 = points[i].Point.X - points[i - 1].Point.X;
				double d2 = points[i + 1].Point.X - points[i - 1].Point.X;
				double d3 = points[i + 1].Point.X - points[i].Point.X;
				double dy1 = points[i + 1].Point.YValues[yIndex] - points[i].Point.YValues[yIndex];
				double dy2 = points[i].Point.YValues[yIndex] - points[i - 1].Point.YValues[yIndex];

				if (points[i].Point.X == points[i - 1].Point.X)
				{
					ys2[i] = 0;
					u[i] = 0;
				}
				else
				{
					p = 1 / (d1 * ys2[i - 1] + 2 * d2);

					ys2[i] = -p * d3;
					u[i] = p * (a * (dy1 / d3 - dy2 / d1) - d1 * u[i - 1]);
				}
			}

			for (int k = count - 2; k >= 0; k--)
				ys2[k] = ys2[k] * ys2[k + 1] + u[k];
		}

		/// <summary>
		/// Given the array of chart points. The procedure returns array of second derivatives of cubic splines at this points.
		/// Then we can get bezier curve coordinates from the second derivatives and points array.
		/// </summary>
		/// <param name="points">The points.</param>
		/// <param name="ys2">The ys2.</param>
		protected void NaturalSpline(ChartStyledPoint[] points, out double[] ys2)
		{
			int count = points.Length;
			int yIndex = m_series.PointFormats[ChartYValueUsage.YValue];

			ys2 = new double[count];

			double a = 6;
			double[] u = new double[count - 1];
			double p;

			ys2[0] = u[0] = 0;
			ys2[count - 1] = 0;

			for (int i = 1; i < count - 1; i++)
			{
				double d1 = points[i].X - points[i - 1].X;
				double d2 = points[i + 1].X - points[i - 1].X;
				double d3 = points[i + 1].X - points[i].X;
				double dy1 = points[i + 1].YValues[yIndex] - points[i].YValues[yIndex];
				double dy2 = points[i].YValues[yIndex] - points[i - 1].YValues[yIndex];

				if (d1 == 0 || d2 == 0 || d3 == 0)
				{
					ys2[i] = 0;
					u[i] = 0;
				}
				else
				{
					p = 1 / (d1 * ys2[i - 1] + 2 * d2);

					ys2[i] = -p * d3;
					u[i] = p * (a * (dy1 / d3 - dy2 / d1) - d1 * u[i - 1]);
				}
			}

			for (int k = count - 2; k >= 0; k--)
				ys2[k] = ys2[k] * ys2[k + 1] + u[k];
		}

		/// <summary>
		/// Gets bezier curve points from cubic spline curve defined by two points and two second derivative y2 at this points.
		/// </summary>
		/// <param name="point1"> Start  of spline curve </param>
		/// <param name="point2"> End  of spline curve</param>
		/// <param name="y2_1"> Second y derivative x at start point </param>
		/// <param name="y2_2"> Second y derivative x at end point </param>
		/// <param name="p0"> First Bezier curve point </param>
		/// <param name="p1"> Second Bezier curve point </param>
		/// <param name="p2"> Third Bezier curve point</param>
		/// <param name="p3"> Fourth Bezier curve point</param>
		protected void BezierPointsFromSpline(PointF point1, PointF point2, float y2_1, float y2_2, out PointF p0, out PointF p1, out PointF p2, out PointF p3)
		{
			float deltaX2 = (point2.X - point1.X);
			deltaX2 = deltaX2 * deltaX2;
			float one_3 = 0.33333f;

			p0 = new PointF(point1.X, point1.Y);

			p1 = new PointF((2 * point1.X + point2.X) * one_3,
				one_3 * (2 * point1.Y + point2.Y - one_3 * deltaX2 * (y2_1 + 0.5f * y2_2)));

			p2 = new PointF((point1.X + 2.0f * point2.X) * one_3,
				one_3 * (point1.Y + 2 * point2.Y - one_3 * deltaX2 * (0.5f * y2_1 + y2_2)));

			p3 = new PointF(point2.X, point2.Y);
		}

		/// <summary>
		/// Gets the bezier control points.
		/// </summary>
		/// <param name="point1">The point1.</param>
		/// <param name="point2">The point2.</param>
		/// <param name="ys1">The ys1.</param>
		/// <param name="ys2">The ys2.</param>
		/// <param name="controlPoint1">The control point1.</param>
		/// <param name="controlPoint2">The control point2.</param>
		/// <param name="yIndex">Index of the y.</param>
		protected void GetBezierControlPoints(ChartStyledPoint point1, ChartStyledPoint point2,
			double ys1, double ys2, out ChartPoint controlPoint1, out ChartPoint controlPoint2, int yIndex)
		{
			const double one_3 = 1 / 3.0d;
			double deltaX2 = (point2.X - point1.X);

			deltaX2 = deltaX2 * deltaX2;

			double dx1 = 2 * point1.X + point2.X;
			double dx2 = point1.X + 2 * point2.X;

			double dy1 = 2 * point1.YValues[yIndex] + point2.YValues[yIndex];
			double dy2 = point1.YValues[yIndex] + 2 * point2.YValues[yIndex];

			double y1 = one_3 * (dy1 - one_3 * deltaX2 * (ys1 + 0.5f * ys2));
			double y2 = one_3 * (dy2 - one_3 * deltaX2 * (0.5f * ys1 + ys2));

			controlPoint1 = new ChartPoint(dx1 * one_3, y1);
			controlPoint2 = new ChartPoint(dx2 * one_3, y2);
		}

		/// <summary>
		/// Gets bezier curve points from cubic spline curve defined by two points and two second derivative y2 at this points.
		/// </summary>
		/// <param name="point1"> Start  of spline curve </param>
		/// <param name="point2"> End  of spline curve</param>
		/// <param name="y2_1"> Second y derivative x at start point </param>
		/// <param name="y2_2"> Second y derivative x at end point </param>
		/// <param name="p0"> First Bezier curve point </param>
		/// <param name="p1"> Second Bezier curve point </param>
		/// <param name="p2"> Third Bezier curve point</param>
		/// <param name="p3"> Fourth Bezier curve point</param>
		protected void BezierPointsFromSpline(ChartPointWithIndex point1, ChartPointWithIndex point2, double y2_1, double y2_2, out ChartPointWithIndex p0, out ChartPointWithIndex p1, out ChartPointWithIndex p2, out ChartPointWithIndex p3)
		{
			double deltaX2 = (point2.Point.X - point1.Point.X);
			deltaX2 = deltaX2 * deltaX2;

			double one_3 = 1 / 3.0d;

			double dx1 = 2 * point1.Point.X + point2.Point.X;
			double dx2 = point1.Point.X + 2 * point2.Point.X;

			double dy1 = 2 * point1.Point.YValues[0] + point2.Point.YValues[0];
			double dy2 = point1.Point.YValues[0] + 2 * point2.Point.YValues[0];

			double x1 = dx1 * one_3;
			double x2 = dx2 * one_3;

			double y1 = one_3 * (dy1 - one_3 * deltaX2 * (y2_1 + 0.5f * y2_2));
			double y2 = one_3 * (dy2 - one_3 * deltaX2 * (0.5f * y2_1 + y2_2));

			p0 = point1;
			p1 = new ChartPointWithIndex(new ChartPoint(x1, y1), point1.Index);
			p2 = new ChartPointWithIndex(new ChartPoint(x2, y2), point2.Index);
			p3 = point2;
		}

		/// <summary>
		/// Given the array of points. The procedure will fit the canonical spline curve to pass through all the points.
		/// Note: The curve will not be "function" line. There can be few Y values for one X value;
		/// </summary>
		/// <param name="points"></param>
		/// <param name="tension">Canonical spline tension</param>
		/// <param name="addextremumpoints"></param>
		/// <param name="bpoints">Bezier points array. The length of this array is 4n, where n is number of intervals (number of points - 1)</param>
		/// <param name="bextrpoints"></param>
		protected void canonicalSpline(ChartPointWithIndex[] points, double tension, bool addextremumpoints, out ChartPointWithIndex[] bpoints, out ChartPointWithIndex[] bextrpoints)
		{
			int n = points.Length;

			ArrayList ba = new ArrayList(4 * (n - 1));

			for (int i = 0; i < n - 1; i++)
			{
				ChartPointWithIndex p0, p1, p2, p3;

				if (i == 0) p0 = points[i];
				else p0 = points[i - 1];

				p1 = points[i];
				p2 = points[i + 1];

				if (i == n - 2) p3 = points[i + 1];
				else p3 = points[i + 2];

				double x1, x2, y1, y2, x0, x3, y0, y3;
				x0 = p0.Point.X;
				x1 = p1.Point.X;
				x2 = p2.Point.X;
				x3 = p3.Point.X;
				y0 = p0.Point.YValues[0];
				y1 = p1.Point.YValues[0];
				y2 = p2.Point.YValues[0];
				y3 = p3.Point.YValues[0];

				double px1, px2, py1, py2, px0, px3, py0, py3;
				px0 = x1;
				py0 = y1;
				px1 = (3.0 * x1 + tension * (x2 - x0)) / 3.0;
				py1 = (3.0 * y1 + tension * (y2 - y0)) / 3.0;
				px2 = (3.0 * x2 - tension * (x3 - x1)) / 3.0;
				py2 = (3.0 * y2 - tension * (y3 - y1)) / 3.0;
				px3 = x2;
				py3 = y2;

				ba.Add(p1);

				ChartPointWithIndex b1 = new ChartPointWithIndex(new ChartPoint(px1, py1), p1.Index);
				ba.Add(b1);

				ChartPointWithIndex b2 = new ChartPointWithIndex(new ChartPoint(px2, py2), p2.Index);
				ba.Add(b2);

				ba.Add(p2);
			}

			bpoints = (ChartPointWithIndex[])ba.ToArray(typeof(ChartPointWithIndex));
			bextrpoints = (ChartPointWithIndex[])ba.ToArray(typeof(ChartPointWithIndex));

			if (addextremumpoints)
			{
				ArrayList baXmin = new ArrayList(4 * (n - 1));
				for (int i = 0; i < ba.Count - 3; i += 4)
				{
					ChartPointWithIndex p0, p1, p2, p3;
					p0 = (ChartPointWithIndex)ba[i];
					p1 = (ChartPointWithIndex)ba[i + 1];
					p2 = (ChartPointWithIndex)ba[i + 2];
					p3 = (ChartPointWithIndex)ba[i + 3];

					double px0, px1, px2, px3;
					px0 = p0.Point.X;
					//py0 = p0.Point.YValues[0];
					px1 = p1.Point.X;
					//py1 = p1.Point.YValues[0];
					px2 = p2.Point.X;
					//py2 = p2.Point.YValues[0];
					px3 = p3.Point.X;
					//py3 = p3.Point.YValues[0];

					double a, b, c, t1, t2;

					a = -px0 + 3 * px1 - 3 * px2 + px3;
					b = 3 * px0 - 6 * px1 + 3 * px2;
					c = -3 * px0 + 3 * px1;

					if (ChartMath.SolveQuadraticEquation(3 * a, 2 * b, c, out t1, out t2))
					{
						#region splitting curves
						if ((t1 > 0) && (t1 < 1))
						{
							ChartPointWithIndex pb0, pb1, pb2, pb3, pe0, pe1, pe2, pe3;

							SplitBezierCurve(p0, p1, p2, p3, t1, out pb0, out pb1, out pb2, out pb3, out pe0, out pe1, out pe2, out pe3);

							baXmin.Add(pb0);
							baXmin.Add(pb1);
							baXmin.Add(pb2);
							baXmin.Add(pb3);

							p0 = pe0;
							p1 = pe1;
							p2 = pe2;
							p3 = pe3;
							t2 = (t2 - t1) / (1 - t1);
						}
						if ((t2 > 0) && (t2 < 1))
						{
							ChartPointWithIndex pb0, pb1, pb2, pb3, pe0, pe1, pe2, pe3;

							SplitBezierCurve(p0, p1, p2, p3, t2, out pb0, out pb1, out pb2, out pb3, out pe0, out pe1, out pe2, out pe3);

							baXmin.Add(pb0);
							baXmin.Add(pb1);
							baXmin.Add(pb2);
							baXmin.Add(pb3);

							p0 = pe0;
							p1 = pe1;
							p2 = pe2;
							p3 = pe3;
						}
						#endregion
					}

					baXmin.Add(p0);
					baXmin.Add(p1);
					baXmin.Add(p2);
					baXmin.Add(p3);
				}


				ArrayList baYmin = new ArrayList(4 * (n - 1));
				for (int i = 0; i < baXmin.Count - 3; i += 4)
				{
					ChartPointWithIndex p0, p1, p2, p3;
					p0 = (ChartPointWithIndex)baXmin[i];
					p1 = (ChartPointWithIndex)baXmin[i + 1];
					p2 = (ChartPointWithIndex)baXmin[i + 2];
					p3 = (ChartPointWithIndex)baXmin[i + 3];

					double py1, py2, py0, py3;
					//px0 = p0.Point.X;
					py0 = p0.Point.YValues[0];
					//px1 = p1.Point.X;
					py1 = p1.Point.YValues[0];
					//px2 = p2.Point.X;
					py2 = p2.Point.YValues[0];
					//px3 = p3.Point.X;
					py3 = p3.Point.YValues[0];

					double a, b, c, t1, t2;

					a = -py0 + 3 * py1 - 3 * py2 + py3;
					b = 3 * py0 - 6 * py1 + 3 * py2;
					c = -3 * py0 + 3 * py1;

					if (ChartMath.SolveQuadraticEquation(3 * a, 2 * b, c, out t1, out t2))
					{
						#region splitting curves
						if ((t1 > 0) && (t1 < 1))
						{
							ChartPointWithIndex pb0, pb1, pb2, pb3, pe0, pe1, pe2, pe3;

							SplitBezierCurve(p0, p1, p2, p3, t1, out pb0, out pb1, out pb2, out pb3, out pe0, out pe1, out pe2, out pe3);

							baYmin.Add(pb0);
							baYmin.Add(pb1);
							baYmin.Add(pb2);
							baYmin.Add(pb3);

							p0 = pe0;
							p1 = pe1;
							p2 = pe2;
							p3 = pe3;
							t2 = (t2 - t1) / (1 - t1);
						}
						if ((t2 > 0) && (t2 < 1))
						{
							ChartPointWithIndex pb0, pb1, pb2, pb3, pe0, pe1, pe2, pe3;

							SplitBezierCurve(p0, p1, p2, p3, t2, out pb0, out pb1, out pb2, out pb3, out pe0, out pe1, out pe2, out pe3);

							baYmin.Add(pb0);
							baYmin.Add(pb1);
							baYmin.Add(pb2);
							baYmin.Add(pb3);

							p0 = pe0;
							p1 = pe1;
							p2 = pe2;
							p3 = pe3;
						}
						#endregion
					}

					baYmin.Add(p0);
					baYmin.Add(p1);
					baYmin.Add(p2);
					baYmin.Add(p3);
				}

				bextrpoints = (ChartPointWithIndex[])baYmin.ToArray(typeof(ChartPointWithIndex));
			}
		}

        /// <summary>
        /// Splits the bezier curve.
        /// </summary>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="t0">The t0.</param>
        /// <param name="pb0">The PB0.</param>
        /// <param name="pb1">The PB1.</param>
        /// <param name="pb2">The PB2.</param>
        /// <param name="pb3">The PB3.</param>
        /// <param name="pe0">The pe0.</param>
        /// <param name="pe1">The pe1.</param>
        /// <param name="pe2">The pe2.</param>
        /// <param name="pe3">The pe3.</param>
		protected void SplitBezierCurve(ChartPointWithIndex p0, ChartPointWithIndex p1, ChartPointWithIndex p2, ChartPointWithIndex p3, double t0,
			out ChartPointWithIndex pb0, out ChartPointWithIndex pb1, out ChartPointWithIndex pb2, out ChartPointWithIndex pb3,
			out ChartPointWithIndex pe0, out ChartPointWithIndex pe1, out ChartPointWithIndex pe2, out ChartPointWithIndex pe3)
		{
			int n = 4;
			double[,] x = new double[n, n];
			double[,] y = new double[n, n];

			x[0, 0] = p0.Point.X;
			x[1, 0] = p1.Point.X;
			x[2, 0] = p2.Point.X;
			x[3, 0] = p3.Point.X;

			y[0, 0] = p0.Point.YValues[0];
			y[1, 0] = p1.Point.YValues[0];
			y[2, 0] = p2.Point.YValues[0];
			y[3, 0] = p3.Point.YValues[0];

			for (int i = 1; i < n; i++)
			{
				for (int j = 0; j < n - i; j++)
				{
					x[j, i] = x[j, i - 1] * (1 - t0) + x[j + 1, i - 1] * t0;
					y[j, i] = y[j, i - 1] * (1 - t0) + y[j + 1, i - 1] * t0;
				}
			}

			pb0 = new ChartPointWithIndex(new ChartPoint(x[0, 0], y[0, 0]), p0.Index);
			pb1 = new ChartPointWithIndex(new ChartPoint(x[0, 1], y[0, 1]), p0.Index);
			pb2 = new ChartPointWithIndex(new ChartPoint(x[0, 2], y[0, 2]), p0.Index);
			pb3 = new ChartPointWithIndex(new ChartPoint(x[0, 3], y[0, 3]), p0.Index);

			pe0 = new ChartPointWithIndex(new ChartPoint(x[0, 3], y[0, 3]), p3.Index);
			pe1 = new ChartPointWithIndex(new ChartPoint(x[1, 2], y[1, 2]), p3.Index);
			pe2 = new ChartPointWithIndex(new ChartPoint(x[2, 1], y[2, 1]), p3.Index);
			pe3 = new ChartPointWithIndex(new ChartPoint(x[3, 0], y[3, 0]), p3.Index);
		}

		/// <summary>
		/// Draws beziers curve.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> to render curve.</param>
		/// <param name="drawpoints">The array of <see cref="System.Drawing.PointF"/> to render curve.</param>
		/// <param name="fillpoints">The array of <see cref="System.Drawing.PointF"/> to fill.</param>
		/// <param name="offset">The curve offset.</param>
		/// <param name="brush">The <see cref="Syncfusion.Drawing.BrushInfo"/> to fill curve body.</param>
		/// <param name="pen">The <see cref="System.Drawing.Pen"/> to render curve border.</param>
		/// <returns><see cref="System.Drawing.Region"/> that represent curve.</returns>
		protected virtual Region Draw3DBeziers(Graphics g, PointF[] drawpoints, PointF[] fillpoints, SizeF offset, BrushInfo brush, Pen pen)
		{
			Region rgn = new Region();
			GraphicsPath top = new GraphicsPath();
			BrushInfo tBrush = brush.Clone();
			{
				for (int i = 0; i < fillpoints.Length - 3; i += 4)
				{
					PointF p0, p1, p2, p3;
					p0 = fillpoints[i];
					p1 = fillpoints[i + 1];
					p2 = fillpoints[i + 2];
					p3 = fillpoints[i + 3];

					top.AddBezier(p0, p1, p2, p3);

					top.AddBezier(new PointF(p3.X + offset.Width, p3.Y + offset.Height),
						new PointF(p2.X + offset.Width, p2.Y + offset.Height),
						new PointF(p1.X + offset.Width, p1.Y + offset.Height),
						new PointF(p0.X + offset.Width, p0.Y + offset.Height));

					if (i == 0)
					{
						rgn = new Region(top);
					}
					else
					{
						rgn.Union(top);
					}

					BrushPaint.FillPath(g, top, tBrush);
					top.Reset();
				}
				for (int i = 0; i < drawpoints.Length - 3; i += 4)
				{
					PointF p0, p1, p2, p3;
					p0 = drawpoints[i];
					p1 = drawpoints[i + 1];
					p2 = drawpoints[i + 2];
					p3 = drawpoints[i + 3];

					top.AddBezier(p0, p1, p2, p3);

					top.AddBezier(new PointF(p3.X + offset.Width, p3.Y + offset.Height),
						new PointF(p2.X + offset.Width, p2.Y + offset.Height),
						new PointF(p1.X + offset.Width, p1.Y + offset.Height),
						new PointF(p0.X + offset.Width, p0.Y + offset.Height));

					g.DrawPath(pen, top);
					top.Reset();
				}
			}
			return rgn;
		}
		#endregion

		#region Draw 3D Cylinder
		/// <summary>
		/// Creates the vertical cylinder 3D geometry.
		/// </summary>
		/// <param name="rect">The bounds of the cylinder.</param>
		/// <param name="offset">The offset.</param>
		/// <returns></returns>
		protected GraphicsPath CreateVerticalCylinder3D(RectangleF rect, SizeF offset)
		{
			GraphicsPath gp = new GraphicsPath();
			float w = rect.Width;
			float offX = offset.Width;
			float offY = offset.Height;

			PointF[] topSide = new PointF[]{ new PointF( rect.Left + 0.5f*w, rect.Top ),
                                       new PointF( rect.Left + 0.75f*w, rect.Top ),
                                       new PointF( rect.Right + 0.25f*offX, rect.Top + 0.25f*offY ),
                                       new PointF( rect.Right + 0.5f*offX, rect.Top + 0.5f*offY ),
                                       new PointF( rect.Right + 0.75f*offX, rect.Top + 0.75f*offY ),
                                       new PointF( rect.Left + 0.75f*w + offX, rect.Top + offY ),
                                       new PointF( rect.Left + 0.5f*w + offX, rect.Top + offY ),
                                       new PointF( rect.Left + 0.25f*w + offX, rect.Top + offY ),
                                       new PointF( rect.Left + 0.75f*offX, rect.Top + 0.75f*offY ),
                                       new PointF( rect.Left + 0.5f*offX, rect.Top + 0.5f*offY ),
                                       new PointF( rect.Left + 0.25f*offX, rect.Top + 0.25f*offY ),
                                       new PointF( rect.Left + 0.25f*w, rect.Top ),
                                       new PointF( rect.Left + 0.5f*w, rect.Top )};

			PointF[] bottomSide = new PointF[]{ new PointF( rect.Left + 0.5f*w, rect.Bottom ),
                                          new PointF( rect.Left + 0.75f*w, rect.Bottom ),
                                          new PointF( rect.Right + 0.25f*offX, rect.Bottom + 0.25f*offY ),
                                          new PointF( rect.Right + 0.5f*offX, rect.Bottom + 0.5f*offY ),
                                          new PointF( rect.Right + 0.75f*offX, rect.Bottom + 0.75f*offY ),
                                          new PointF( rect.Left + 0.75f*w + offX, rect.Bottom + offY ),
                                          new PointF( rect.Left + 0.5f*w + offX, rect.Bottom + offY ),
                                          new PointF( rect.Left + 0.25f*w + offX, rect.Bottom + offY ),
                                          new PointF( rect.Left + 0.75f*offX, rect.Bottom + 0.75f*offY ),
                                          new PointF( rect.Left + 0.5f*offX, rect.Bottom + 0.5f*offY ),
                                          new PointF( rect.Left + 0.25f*offX, rect.Bottom + 0.25f*offY ),
                                          new PointF( rect.Left + 0.25f*w, rect.Bottom ),
                                          new PointF( rect.Left + 0.5f*w, rect.Bottom )};


			PointF[] topFrontSide = new PointF[]{ topSide[9], topSide[10], topSide[11], topSide[0], topSide[1], 
                                            topSide[2], topSide[3], topSide[4], topSide[5], topSide[6] };
			PointF[] bottomFrontSide = new PointF[]{ bottomSide[6], bottomSide[5], bottomSide[4], bottomSide[3], bottomSide[2], 
                                               bottomSide[1], bottomSide[0], bottomSide[11], bottomSide[10], bottomSide[9] };

			GetLeftBezierPoint(topFrontSide[3], ref topFrontSide[2], ref topFrontSide[1], ref topFrontSide[0]);
			GetRightBezierPoint(topFrontSide[6], ref topFrontSide[7], ref topFrontSide[8], ref topFrontSide[9]);
			GetLeftBezierPoint(bottomFrontSide[6], ref bottomFrontSide[7], ref bottomFrontSide[8], ref bottomFrontSide[9]);
			GetRightBezierPoint(bottomFrontSide[3], ref bottomFrontSide[2], ref bottomFrontSide[1], ref bottomFrontSide[0]);

			gp.StartFigure();
			gp.AddBeziers(topFrontSide);
			gp.AddBeziers(bottomFrontSide);
			gp.CloseFigure();

			gp.AddBeziers(topSide);

			return gp;
		}
        /// <summary>
        /// Creates the horizontal cylinder 3D top geometry.
        /// </summary>
        /// <param name="rect">The bounds of the cylinder.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>
        /// 	<see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent cylinder.
        /// </returns>
        protected GraphicsPath CreateHorizintalCylinder3DTop(RectangleF rect, SizeF offset)
        {
            GraphicsPath gp = new GraphicsPath();
            float h = rect.Height;
            float offX = offset.Width;
            float offY = offset.Height;

            PointF[] topSide = new PointF[]{ new PointF( rect.Right, rect.Top  + 0.5f*h ),
                                       new PointF( rect.Right, rect.Top  + 0.75f*h ),
                                       new PointF( rect.Right  + 0.25f*offX, rect.Bottom + 0.25f*offY ),
                                       new PointF( rect.Right + 0.5f*offX, rect.Bottom + 0.5f*offY ),
                                       new PointF( rect.Right + 0.75f*offX, rect.Bottom + 0.75f*offY ),
                                       new PointF( rect.Right + offX, rect.Top + 0.75f*h + offY ),
                                       new PointF( rect.Right + offX, rect.Top + 0.5f*h + offY ),
                                       new PointF( rect.Right + offX, rect.Top + 0.25f*h + offY ),
                                       new PointF( rect.Right + 0.75f*offX, rect.Top + 0.75f*offY ),
                                       new PointF( rect.Right + 0.5f*offX, rect.Top + 0.5f*offY ),
                                       new PointF( rect.Right + 0.25f*offX, rect.Top + 0.25f*offY ),
                                       new PointF( rect.Right, rect.Top + 0.25f*h ),
                                       new PointF( rect.Right, rect.Top + 0.5f*h )};
            gp.AddBeziers(topSide);

            return gp;
        }
        /// <summary>
        /// Creates the vertical cylinder 3D top geometry.
        /// </summary>
        /// <param name="rect">The bounds of the cylinder.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        protected GraphicsPath CreateVerticalCylinder3DTop(RectangleF rect, SizeF offset)
        {

            GraphicsPath gp = new GraphicsPath();
            float w = rect.Width;
            float offX = offset.Width;
            float offY = offset.Height;

            PointF[] topSide = new PointF[]{ new PointF( rect.Left + 0.5f*w, rect.Top ),
                                       new PointF( rect.Left + 0.75f*w, rect.Top ),
                                       new PointF( rect.Right + 0.25f*offX, rect.Top + 0.25f*offY ),
                                       new PointF( rect.Right + 0.5f*offX, rect.Top + 0.5f*offY ),
                                       new PointF( rect.Right + 0.75f*offX, rect.Top + 0.75f*offY ),
                                       new PointF( rect.Left + 0.75f*w + offX, rect.Top + offY ),
                                       new PointF( rect.Left + 0.5f*w + offX, rect.Top + offY ),
                                       new PointF( rect.Left + 0.25f*w + offX, rect.Top + offY ),
                                       new PointF( rect.Left + 0.75f*offX, rect.Top + 0.75f*offY ),
                                       new PointF( rect.Left + 0.5f*offX, rect.Top + 0.5f*offY ),
                                       new PointF( rect.Left + 0.25f*offX, rect.Top + 0.25f*offY ),
                                       new PointF( rect.Left + 0.25f*w, rect.Top ),
                                       new PointF( rect.Left + 0.5f*w, rect.Top )};


            gp.AddBeziers(topSide);

            return gp;
        }
		/// <summary>
		/// Creates the horizontal cylinder 3D geometry.
		/// </summary>
		/// <param name="rect">The bounds of the cylinder.</param>
		/// <param name="offset">The offset.</param>
		/// <returns>
		/// 	<see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent cylinder.
		/// </returns>
		protected GraphicsPath CreateHorizintalCylinder3D(RectangleF rect, SizeF offset)
		{
			GraphicsPath gp = new GraphicsPath();
			float h = rect.Height;
			float offX = offset.Width;
			float offY = offset.Height;

			PointF[] topSide = new PointF[]{ new PointF( rect.Right, rect.Top  + 0.5f*h ),
                                       new PointF( rect.Right, rect.Top  + 0.75f*h ),
                                       new PointF( rect.Right  + 0.25f*offX, rect.Bottom + 0.25f*offY ),
                                       new PointF( rect.Right + 0.5f*offX, rect.Bottom + 0.5f*offY ),
                                       new PointF( rect.Right + 0.75f*offX, rect.Bottom + 0.75f*offY ),
                                       new PointF( rect.Right + offX, rect.Top + 0.75f*h + offY ),
                                       new PointF( rect.Right + offX, rect.Top + 0.5f*h + offY ),
                                       new PointF( rect.Right + offX, rect.Top + 0.25f*h + offY ),
                                       new PointF( rect.Right + 0.75f*offX, rect.Top + 0.75f*offY ),
                                       new PointF( rect.Right + 0.5f*offX, rect.Top + 0.5f*offY ),
                                       new PointF( rect.Right + 0.25f*offX, rect.Top + 0.25f*offY ),
                                       new PointF( rect.Right, rect.Top + 0.25f*h ),
                                       new PointF( rect.Right, rect.Top + 0.5f*h )};

			PointF[] bottomSide = new PointF[]{ new PointF( rect.Left, rect.Top  + 0.5f*h ),
                                          new PointF( rect.Left, rect.Top  + 0.75f*h ),
                                          new PointF( rect.Left  + 0.25f*offX, rect.Bottom + 0.25f*offY ),
                                          new PointF( rect.Left + 0.5f*offX, rect.Bottom + 0.5f*offY ),
                                          new PointF( rect.Left + 0.75f*offX, rect.Bottom + 0.75f*offY ),
                                          new PointF( rect.Left + offX, rect.Top + 0.75f*h + offY ),
                                          new PointF( rect.Left + offX, rect.Top + 0.5f*h + offY ),
                                          new PointF( rect.Left + offX, rect.Top + 0.25f*h + offY ),
                                          new PointF( rect.Left + 0.75f*offX, rect.Top + 0.75f*offY ),
                                          new PointF( rect.Left + 0.5f*offX, rect.Top + 0.5f*offY ),
                                          new PointF( rect.Left + 0.25f*offX, rect.Top + 0.25f*offY ),
                                          new PointF( rect.Left, rect.Top + 0.25f*h ),
                                          new PointF( rect.Left, rect.Top + 0.5f*h )};


			PointF[] topFrontSide = new PointF[]{ topSide[6], topSide[7], topSide[8], topSide[9], topSide[10], 
                                            topSide[11], topSide[0], topSide[1], topSide[2], topSide[3] };
			PointF[] bottomFrontSide = new PointF[]{ bottomSide[3], bottomSide[2], bottomSide[1], bottomSide[0], bottomSide[11], 
                                               bottomSide[10], bottomSide[9], bottomSide[8], bottomSide[7], bottomSide[6] };

			GetTopBezierPoint(topFrontSide[3], ref topFrontSide[2], ref topFrontSide[1], ref topFrontSide[0]);
			GetBottomBezierPoint(topFrontSide[6], ref topFrontSide[7], ref topFrontSide[8], ref topFrontSide[9]);
			GetTopBezierPoint(bottomFrontSide[6], ref bottomFrontSide[7], ref bottomFrontSide[8], ref bottomFrontSide[9]);
			GetBottomBezierPoint(bottomFrontSide[3], ref bottomFrontSide[2], ref bottomFrontSide[1], ref bottomFrontSide[0]);

			gp.StartFigure();
			gp.AddBeziers(topFrontSide);
			gp.AddBeziers(bottomFrontSide);
			gp.CloseFigure();

			gp.AddBeziers(topSide);

			return gp;
		}

		/// <summary>
		/// Gets the left bezier point.
		/// </summary>
		/// <param name="p1">The start point.</param>
		/// <param name="p2">The first control point.</param>
		/// <param name="p3">The second control point.</param>
		/// <param name="p4">The end point.</param>
		private void GetLeftBezierPoint(PointF p1, ref PointF p2, ref PointF p3, ref PointF p4)
		{
			float cx = 3 * (p2.X - p1.X);
			float cy = 3 * (p2.Y - p1.Y);

			float bx = 3 * (p3.X - p2.X) - cx;
			float by = 3 * (p3.Y - p2.Y) - cy;

			float ax = p4.X - p1.X - bx - cx;
			float ay = p4.Y - p1.Y - by - cy;

			float r1, r2;
			ChartMath.SolveQuadraticEquation(3 * ax, 2 * bx, cx, out r1, out r2);

			PointF pe0, pe1, pe2, pe3;

			if (r1 > 0 && r1 < 1)
			{
				ChartMath.SplitBezierCurve(p1, p2, p3, p4, r1, out p1, out p2, out p3, out p4,
					out pe0, out pe1, out pe2, out pe3);
			}

			if (r2 > 0 && r2 < 1)
			{
				ChartMath.SplitBezierCurve(p1, p2, p3, p4, r2, out p1, out p2, out p3, out p4,
					out pe0, out pe1, out pe2, out pe3);
			}
		}

		/// <summary>
		/// Gets the right bezier point.
		/// </summary>
		/// <param name="p1">The start point.</param>
		/// <param name="p2">The first control point.</param>
		/// <param name="p3">The second control point.</param>
		/// <param name="p4">The end point.</param>
		private void GetRightBezierPoint(PointF p1, ref PointF p2, ref PointF p3, ref PointF p4)
		{
			float cx = 3 * (p2.X - p1.X);
			float cy = 3 * (p2.Y - p1.Y);

			float bx = 3 * (p3.X - p2.X) - cx;
			float by = 3 * (p3.Y - p2.Y) - cy;

			float ax = p4.X - p1.X - bx - cx;
			float ay = p4.Y - p1.Y - by - cy;

			float r1, r2;
			ChartMath.SolveQuadraticEquation(3 * ax, 2 * bx, cx, out r1, out r2);
			PointF pe0, pe1, pe2, pe3;

			if (r1 > 0 && r1 < 1)
			{
				ChartMath.SplitBezierCurve(p1, p2, p3, p4, r1, out p1, out p2, out p3, out p4,
					out pe0, out pe1, out pe2, out pe3);
			}

			if (r2 > 0 && r2 < 1)
			{
				ChartMath.SplitBezierCurve(p1, p2, p3, p4, r2, out p1, out p2, out p3, out p4,
					out pe0, out pe1, out pe2, out pe3);
			}
		}

		/// <summary>
		/// Gets the top bezier point.
		/// </summary>
		/// <param name="p1">The start point.</param>
		/// <param name="p2">The first control point.</param>
		/// <param name="p3">The second control point.</param>
		/// <param name="p4">The end point.</param>
		private void GetTopBezierPoint(PointF p1, ref PointF p2, ref PointF p3, ref PointF p4)
		{
			float cx = 3 * (p2.X - p1.X);
			float cy = 3 * (p2.Y - p1.Y);

			float bx = 3 * (p3.X - p2.X) - cx;
			float by = 3 * (p3.Y - p2.Y) - cy;

			float ax = p4.X - p1.X - bx - cx;
			float ay = p4.Y - p1.Y - by - cy;

			float r1, r2;
			ChartMath.SolveQuadraticEquation(3 * ay, 2 * by, cy, out r1, out r2);
			PointF pe0, pe1, pe2, pe3;

			if (r1 > 0 && r1 < 1)
			{
				ChartMath.SplitBezierCurve(p1, p2, p3, p4, r1, out p1, out p2, out p3, out p4,
					out pe0, out pe1, out pe2, out pe3);
			}

			if (r2 > 0 && r2 < 1)
			{
				ChartMath.SplitBezierCurve(p1, p2, p3, p4, r2, out p1, out p2, out p3, out p4,
					out pe0, out pe1, out pe2, out pe3);
			}
		}

		/// <summary>
		/// Gets the bottom bezier point.
		/// </summary>
		/// <param name="p1">The start point.</param>
		/// <param name="p2">The first control point.</param>
		/// <param name="p3">The second control point.</param>
		/// <param name="p4">The end point.</param>
		private void GetBottomBezierPoint(PointF p1, ref PointF p2, ref PointF p3, ref PointF p4)
		{
			float cx = 3 * (p2.X - p1.X);
			float cy = 3 * (p2.Y - p1.Y);

			float bx = 3 * (p3.X - p2.X) - cx;
			float by = 3 * (p3.Y - p2.Y) - cy;

			float ax = p4.X - p1.X - bx - cx;
			float ay = p4.Y - p1.Y - by - cy;

			float r1, r2;
			ChartMath.SolveQuadraticEquation(3 * ay, 2 * by, cy, out r1, out r2);
			PointF pe0, pe1, pe2, pe3;

			if (r1 > 0 && r1 < 1)
			{
				ChartMath.SplitBezierCurve(p1, p2, p3, p4, r1, out p1, out p2, out p3, out p4,
					out pe0, out pe1, out pe2, out pe3);
			}

			if (r2 > 0 && r2 < 1)
			{
				ChartMath.SplitBezierCurve(p1, p2, p3, p4, r2, out p1, out p2, out p3, out p4,
					out pe0, out pe1, out pe2, out pe3);
			}
		}
		#endregion

		#region Draw 3D Line
		/// <summary>
		/// Draw 3D lines.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> to render lines.</param>
		/// <param name="points">The lines' points.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="brush">The <see cref="Syncfusion.Drawing.BrushInfo"/> to fill lines body.</param>
		/// <param name="pen">The <see cref="System.Drawing.Pen"/> to render lines border.</param>
		/// <param name="colors">The array of <see cref="System.Drawing.Color"/> to draw lines.</param>
		/// <returns><see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent lines.</returns>
		protected virtual Region Draw3DLines(Graphics g, PointF[] points, SizeF offset, BrushInfo brush, Pen pen, Color[] colors)
		{
			Region rgn = new Region();
			GraphicsPath top = new GraphicsPath();
			BrushInfo tBrush = brush.Clone();
			{
				int start;
				int i = start = 0, di = 1, mi = points.Length - 1;
				if (XAxis.Inversed)
				{
					i = start = points.Length - 2;
					di = -1;
					mi = -1;
				}
				for (; i != mi; i += di)
				{
					top.AddPolygon(new PointF[]
          {
            points[i],
            points[i + 1],
            new PointF( points[i + 1].X + offset.Width, points[i + 1].Y + offset.Height ),
            new PointF( points[i].X + offset.Width, points[i].Y + offset.Height )
          });

					if (i == start)
					{
						rgn = new Region(top);
					}
					else
					{
						rgn.Union(top);
					}

					tBrush = new BrushInfo(colors[i]);

					BrushPaint.FillPath(g, top, tBrush);
					g.DrawPath(pen, top);
					top.Reset();
				}
			}
			return rgn;
		}

		/// <summary>
		/// Draw 3D lines.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> to render lines.</param>
		/// <param name="points">The lines' points.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="brush">The <see cref="Syncfusion.Drawing.BrushInfo"/> to fill lines body.</param>
		/// <param name="pen">The <see cref="System.Drawing.Pen"/> to render lines border.</param>
		/// <returns><see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent lines.</returns>
		protected virtual Region Draw3DLines(Graphics g, PointF[] points, SizeF offset, BrushInfo brush, Pen pen)
		{
			Region rgn = new Region();
			GraphicsPath top = new GraphicsPath();
			{
				int start;
				int i = start = 0, di = 1, mi = points.Length - 1;
				if (XAxis.Inversed)
				{
					i = start = points.Length - 2;
					di = -1;
					mi = -1;
				}
				for (; i != mi; i += di)
				{
					top.AddPolygon(new PointF[]
          {
            points[i],
            points[i + 1],
            new PointF( points[i + 1].X + offset.Width, points[i + 1].Y + offset.Height ),
            new PointF( points[i].X + offset.Width, points[i].Y + offset.Height )
          });

					if (i == start)
					{
						rgn = new Region(top);
					}
					else
					{
						rgn.Union(top);
					}
					if (m_series.Type == ChartSeriesType.Line)
						brush = GetBrush(i);
					BrushPaint.FillPath(g, top, brush);
					g.DrawPath(pen, top);
					top.Reset();
				}
			}
			return rgn;
		}
        /// <summary>
        /// Draw for given Graphical path.
        /// </summary>
        /// <param name="cg">Chart Graph.</param>
        /// <param name="gp">Graphical Path.</param>
        ///  <param name=" br">BrushInfo.</param>
        ///   <param name="p">Pen.</param>
        public void Draw(ChartGraph cg, GraphicsPath gp, BrushInfo br, Pen p)
        {

            if (cg != null)
            {
                ColorBlend c_cylinderPhong;
                Color[] colors;
                float[] positions;

                ColorBlend colorBlend = new ColorBlend();

                ChartSeriesRenderer.PhongShadingColors(Color.FromArgb(200, br.BackColor), Color.FromArgb(0x90, br.BackColor), Color.FromArgb(100, Color.Black), Math.PI / 4, 30, out colors, out positions);

                colorBlend.Positions = positions;
                colorBlend.Colors = colors;
                c_cylinderPhong = colorBlend;

                cg.DrawPath(br, p, gp);

                using (LinearGradientBrush lgb = new LinearGradientBrush(new Rectangle(0, 0, 1, 1),
                Color.Black, Color.White, LinearGradientMode.Vertical))
                {
                    lgb.InterpolationColors = c_cylinderPhong;

                    cg.DrawPath(lgb, p, gp);
                }
            }
        }
		#endregion

		#region Draw rectangle
		/// <summary>
		/// Calculates <see cref="System.Drawing.RectangleF"/> for given chart points.
		/// </summary>
		/// <param name="firstPoint">The first chart point to calculate rectangle.</param>
		/// <param name="secondPoint">The second chart point to calculate rectangle.</param>
		/// <returns>Calculated rectangle.</returns>
		protected virtual RectangleF GetRectangle(ChartPoint firstPoint, ChartPoint secondPoint)
		{
			PointF firstPointF = new PointF(this.GetXFromValue(firstPoint, 0), this.GetYFromValue(firstPoint, 0));
			PointF secondPointF = new PointF(this.GetXFromValue(secondPoint, 0), this.GetYFromValue(secondPoint, 0));
			return new RectangleF(
				Math.Min(firstPointF.X, secondPointF.X),
				Math.Min(firstPointF.Y, secondPointF.Y),
				Math.Abs(secondPointF.X - firstPointF.X),
				Math.Abs(secondPointF.Y - firstPointF.Y));
		}

		/// <summary>
		/// Helper method to render a 3D rectangle.
		/// </summary>
		/// <param name="g">The graphics object that is to be used for rendering.</param>
		/// <param name="rc">The rectangle that is to be drawn.</param>
		/// <param name="offset">The Offset in 3D.</param>
		/// <param name="brush">The brush that is to be used for filling the rectangle sides.</param>
		/// <param name="pen">The pen that is to be used for drawing the rectangle sides.</param>
		protected virtual Region Draw3DRectangle(Graphics g, RectangleF rc, SizeF offset, BrushInfo brush, Pen pen)
		{
			GraphicsPath gp = new GraphicsPath();

			//Side
			gp.AddPolygon(new PointF[]
        {
          new PointF( rc.Right, rc.Top ),
          new PointF( rc.Right + offset.Width, rc.Top + offset.Height ),
          new PointF( rc.Right + offset.Width, rc.Bottom + offset.Height ),
          new PointF( rc.Right, rc.Bottom )
        });
			Region rgn = new Region(gp);
			BrushPaint.FillPath(g, gp, brush);

			if (EnableStyles)
			{
				g.DrawPath(pen, gp);
			}

			//Top
			gp.Reset();
			gp.AddPolygon(new PointF[]
        {
          new PointF( rc.X, rc.Top ), 
          new PointF( rc.X + offset.Width, rc.Top + offset.Height ),
          new PointF( rc.X + offset.Width + rc.Width, rc.Top + offset.Height ),
          new PointF( rc.Right, rc.Top )
        });

			rgn.Union(gp);
			BrushPaint.FillPath(g, gp, brush);

			if (EnableStyles)
			{
				g.DrawPath(pen, gp);
			}

			//Front side
			gp.Reset();
			gp.AddPolygon(new PointF[]
        {
          new PointF( rc.Left, rc.Top ),
          new PointF( rc.Right, rc.Top ),
          new PointF( rc.Right, rc.Bottom),
          new PointF( rc.Left, rc.Bottom )
        });
			rgn.Union(gp);
			BrushPaint.FillPath(g, gp, brush);

			if (EnableStyles)
			{
				g.DrawPath(pen, gp);
			}

			return rgn;
		}

		/// <summary>
		/// Creates <see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent box.
		/// </summary>
		/// <param name="rect">The bounds of the box.</param>
		/// <param name="is3D">The value indicates that box is in 3D.</param>
		/// <returns><see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent box.</returns>
		protected virtual GraphicsPath CreateBox(RectangleF rect, bool is3D)
		{
			GraphicsPath gp = new GraphicsPath();
			SizeF szOffset = GetSeriesOffset();
			//rect = GetRectByClip( rect );

			//if (!rect.IsEmpty)
			{
				gp.AddRectangle(rect);

				if (is3D)
				{
					gp.AddPolygon(new PointF[]{ new PointF( rect.Left, rect.Top ),
                                       new PointF( rect.Left + szOffset.Width, rect.Top + szOffset.Height ),
                                       new PointF( rect.Right + szOffset.Width, rect.Top + szOffset.Height ),
                                       new PointF( rect.Right, rect.Top )});
					gp.AddPolygon(new PointF[]{ new PointF( rect.Right, rect.Top ),
                                       new PointF( rect.Right + szOffset.Width, rect.Top + szOffset.Height ),
                                       new PointF( rect.Right + szOffset.Width, rect.Bottom + szOffset.Height ),
                                       new PointF( rect.Right, rect.Bottom )});
				}
			}

			return gp;
		}
        /// <summary>
        /// Creates <see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent box.
        /// </summary>
        /// <param name="rect">The bounds of the box.</param>
        /// <param name="is3D">The value indicates that box is in 3D.</param>
        /// <returns><see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent box.</returns>
        protected virtual GraphicsPath CreateBoxRight(RectangleF rect, bool is3D)
        {
            GraphicsPath gp = new GraphicsPath();
            SizeF szOffset = GetSeriesOffset();
            

              
                if (is3D)
                {
                    gp.AddPolygon(new PointF[]{ new PointF(rect.Left, rect.Top ),
                                       new PointF( rect.Left + szOffset.Width+1, rect.Top + szOffset.Height ),
                                       new PointF( rect.Right + szOffset.Width, rect.Top + szOffset.Height ),
                                       new PointF( rect.Right, rect.Top)});
                    gp.AddPolygon(new PointF[]{ new PointF( rect.Right, rect.Top ),
                                       new PointF( rect.Right + szOffset.Width, rect.Top + szOffset.Height ),
                                       new PointF( rect.Right + szOffset.Width, rect.Bottom + szOffset.Height ),
                                       new PointF( rect.Right, rect.Bottom )});

                }
            

            return gp;
        }
        /// <summary>
        /// Creates <see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent box.
        /// </summary>
        /// <param name="rect">The bounds of the box.</param>
        /// <param name="is3D">The value indicates that box is in 3D.</param>
        /// <returns><see cref="System.Drawing.Drawing2D.GraphicsPath"/> that represent box.</returns>
        protected virtual GraphicsPath CreateBoxTop(RectangleF rect, bool is3D)
        {
            GraphicsPath gp = new GraphicsPath();
            SizeF szOffset = GetSeriesOffset();
            
                if (is3D)
                {
                    gp.AddPolygon(new PointF[]{ new PointF(rect.Left, rect.Top ),
                                       new PointF( rect.Left + szOffset.Width+1, rect.Top + szOffset.Height ),
                                       new PointF( rect.Right + szOffset.Width, rect.Top + szOffset.Height ),
                                       new PointF( rect.Right, rect.Top)});
                }
            return gp;
        }
        
		#endregion

		#region Draw Symbols
		/// <summary>
		/// Renders the symbol that is to be associated with a point. Delegates to the <see cref="RenderingHelper"/> class.
		/// <seealso cref="ChartSymbolInfo"/>
		/// </summary>
		/// <param name="g">The graphics object that is to be used.</param>
		/// <param name="styledPoint">The associated point.</param>
		private void DrawPointSymbol(Graphics3D g, ChartStyledPoint styledPoint)
		{
			ChartStyleInfo cstl = styledPoint.Style;
			ChartSymbolInfo stl = cstl.Symbol;
			Vector3D pt = GetSymbolVector(styledPoint);
			Rectangle rc = new Rectangle((int)(pt.X - stl.Size.Width / 2), (int)(pt.Y - stl.Size.Height / 2),
				stl.Size.Width, stl.Size.Height);

			if (stl.Shape != ChartSymbolShape.Image)
			{
				GraphicsPath gp = ChartSymbolHelper.GetPathSymbol(stl.Shape, rc);

                if (this.Chart.HighlightSymbol)
                {                                     
                    BrushInfo brushInfo = this.GetBrush(styledPoint.Index, cstl.Symbol.Color);
                    Brush brush = ChartGraph.GetBrushItem(brushInfo, rc);                
                    Path3D p3d = Path3D.FromGraphicsPath(gp, pt.Z, brush, cstl.Symbol.Border.GdipPen);
                    p3d.RegionData = new ChartRegionData(m_chart.Series.IndexOf(m_series), styledPoint.Index, styledPoint.ToolTip, "Symbol");
                    g.AddPolygon(p3d);                    
                }
                else
                {                  
                    Path3D p3d = Path3D.FromGraphicsPath(gp, pt.Z, new SolidBrush(stl.Color), cstl.Symbol.Border.GdipPen);
                    p3d.RegionData = new ChartRegionData(m_chart.Series.IndexOf(m_series), styledPoint.Index, styledPoint.ToolTip, "Symbol");
                    g.AddPolygon(p3d);
                }
			}
			else if (stl.ImageIndex >= 0)
			{
				Image3D img = Image3D.FromImage(cstl.Images[stl.ImageIndex], rc, (float)pt.Z);
				img.RegionData = new ChartRegionData(m_chart.Series.IndexOf(m_series), styledPoint.Index, styledPoint.ToolTip, "Symbol");
				g.AddPolygon(img);
			}
		}

		/// <summary>
		/// Renders the symbol that is to be associated with a point. Delegates to the <see cref="RenderingHelper"/> class.
		/// <seealso cref="ChartSymbolInfo"/>
		/// </summary>
		/// <param name="g">The graphics object that is to be used.</param>
		/// <param name="style">The style that is to be used.</param>
		/// <param name="pt">Anchor point.</param>
		/// <param name="drawMarker">Indicates whether a marker should be drawn.</param>
		internal virtual void DrawPointSymbol(Graphics3D g, ChartStyleInfo style, Vector3D pt, bool drawMarker)
		{
			ChartStyleInfo cstl = style;
			ChartSymbolInfo stl = cstl.Symbol;
			Rectangle rc = new Rectangle((int)(pt.X - stl.Size.Width / 2), (int)(pt.Y - stl.Size.Height / 2),
				stl.Size.Width, stl.Size.Height);

			if (stl.Shape != ChartSymbolShape.Image)
			{
				GraphicsPath gp = ChartSymbolHelper.GetPathSymbol(stl.Shape, rc);
				Path3D p3d = Path3D.FromGraphicsPath(gp, pt.Z, new SolidBrush(stl.Color), cstl.Symbol.Border.GdipPen);
				p3d.RegionData = new ChartRegionData(GetToolTip(), "Symbol");
				g.AddPolygon(p3d);
			}
			else if (stl.ImageIndex >= 0)
			{
				Image3D img = Image3D.FromImage(cstl.Images[stl.ImageIndex], rc, (float)pt.Z);
				img.RegionData = new ChartRegionData(GetToolTip(), "Symbol");
				g.AddPolygon(img);
			}
		}

		/// <summary>
		/// Adds the symbol region by the specified point index.
		/// </summary>
		/// <param name="styledPoint">The associated point.</param>
		protected virtual void AddSymbolRegion(ChartStyledPoint styledPoint)
		{
			PointF pt = this.GetSymbolPoint(styledPoint);
			ChartStyleInfo style = styledPoint.Style;
			RectangleF rect = new RectangleF(pt.X - style.Symbol.Size.Width / 2,
				pt.Y - style.Symbol.Size.Height / 2,
				style.Symbol.Size.Width,
				style.Symbol.Size.Height);
			m_chart.ChartRegions.Add(new ChartRegion(new Region(rect),
				m_chart.Series.IndexOf(m_series), styledPoint.Index, styledPoint.ToolTip, "Symbol"));
		}

		/// <summary>
		/// Called by several derived renderers to create a region from a 'Hit Test' circle. By overriding this
		/// method you can expand, contract or change this region.
		/// </summary>
		/// <param name="center">The anchor point.</param>
		/// <param name="radius">The radius of the circle that is to be used as the base for the region.</param>
		/// <returns>Region object that is commonly used for hit testing, for display of tooltips and the like.</returns>
		protected virtual Region GetRegionFromCircle(PointF center, float radius)
		{
			GraphicsPath gp = new GraphicsPath();
			gp.AddEllipse(center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
			return new Region(gp);
		}

		/// <summary>
		/// Gets the 3D circle.
		/// </summary>
		/// <param name="pt">The center of circle.</param>
		/// <param name="radius">The radius.</param>
		/// <returns></returns>
		protected virtual Path3D GetPath3DFromCircle(Vector3D pt, float radius)
		{
			GraphicsPath gp = new GraphicsPath();
			gp.AddEllipse((float)pt.X - radius, (float)pt.Y - radius, 2 * radius, 2 * radius);
			return Path3D.FromGraphicsPath(gp, (float)pt.Z, (BrushInfo)null, null);
		}
		#endregion

		#region Get Base Interior
		/// <summary>
		/// Brush information is retrieved from the style associated with the index of the point to be rendered.
		/// It is then changed for special cases such as when automatic highlighting is enabled.
		/// </summary>
		/// <param name="index">Index value of the point for which the brush information is required.</param>
		/// <returns>Brush information that is to be used for filling elements displayed at this index.</returns>
		protected virtual BrushInfo GetBrush(int index)
		{
			ChartStyleInfo style = GetStyleAt(index);
			BrushInfo brush = style.Interior;           
			int serIndex = Chart.Series.IndexOf(m_series);

            if ((!Chart.AutoHighlight) && Chart.SeriesHighlight && (Chart.SeriesHighlightIndex != -1))
            {
                brush = this.GetSeriesHighlightBrush(serIndex, brush, style);
            }

			if (Chart.AutoHighlight && Chart.ActiveIndex >= 0)
			{
				int activeIndex = Chart.ActiveIndex;

				if (index >= 0 && index < this.Chart.ChartRegions.Count)
				{
					ChartRegion rgn = this.Chart.ChartRegions[activeIndex];

					if (rgn.IsChartPoint && rgn.SeriesIndex == serIndex && rgn.PointIndex == index)
					{
						if (style.HighlightInterior == null)
						{
							Color c2 = brush.BackColor;
							c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
							brush = new BrushInfo(c2);
						}
						else
						{
							brush = style.HighlightInterior;
						}
					}                   
				}
			}
            else if (Chart.SeriesHighlight && Chart.ActiveIndex >= 0)
            {
                int activeIndex = Chart.ActiveIndex;

                if (index >= 0 && index < this.Chart.ChartRegions.Count)
                {
                    ChartRegion rgn = this.Chart.ChartRegions[activeIndex];

                    if (rgn.IsChartPoint && rgn.SeriesIndex == serIndex)
                     {
                        if (style.HighlightInterior == null)
                        {
                            Color c2 = brush.BackColor;
                            c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                            brush = new BrushInfo(c2);
                        }
                        else
                        {
                            brush = style.HighlightInterior;                            
                        }
                    }
                    else if (rgn.IsChartPoint && rgn.SeriesIndex != serIndex)
                    {
                        if (style.DimmedInterior == null)
                        {
                            Color c2 = brush.BackColor;
                            c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                            brush = new BrushInfo(c2);
                            brush = new BrushInfo(50, brush);
                        }
                        else
                        {
                            brush = style.DimmedInterior;
                        }
                    }                    
                }
            }       
         
			return brush;
		}

        /// <summary>
        /// Brush information is retrieved from the style associated with the index of the point to be rendered.
        /// It is then changed for special cases such as when automatic highlighting, series highlighting, symbol highlighting are enabled.
        /// </summary>
        /// <param name="index">Index value of the point for which the brush information is required.</param>
        /// <returns>Brush information that is to be used for filling elements displayed at this index.</returns>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        protected virtual BrushInfo GetBrush(int index, Color color)
        {
            ChartStyleInfo style = GetStyleAt(index);
            BrushInfo brush = new BrushInfo(color);
            int serIndex = Chart.Series.IndexOf(m_series);

            if ((!Chart.AutoHighlight) && Chart.SeriesHighlight && (Chart.SeriesHighlightIndex != -1))
            {               
               brush = this.GetSymbolHighlightBrush(serIndex, brush, style);
            }

            if (Chart.AutoHighlight && Chart.ActiveIndex >= 0)
            {
                int activeIndex = Chart.ActiveIndex;

                if (index >= 0 && index < this.Chart.ChartRegions.Count)
                {
                    ChartRegion rgn = this.Chart.ChartRegions[activeIndex];

                    if (rgn.IsChartPoint && rgn.SeriesIndex == serIndex && rgn.PointIndex == index)
                    {
                        if (style.Symbol.HighlightColor==Color.Transparent)
                        {
                            Color c2 = brush.BackColor;
                            c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                            brush = new BrushInfo(c2);
                        }
                        else
                        {
                            brush = new BrushInfo(style.Symbol.HighlightColor);
                        }
                    }
                }
            }
            else if (Chart.SeriesHighlight && Chart.ActiveIndex >= 0)
            {
                int activeIndex = Chart.ActiveIndex;

                if (index >= 0 && index < this.Chart.ChartRegions.Count)
                {
                    ChartRegion rgn = this.Chart.ChartRegions[activeIndex];

                    if (rgn.IsChartPoint && rgn.SeriesIndex == serIndex)
                    {
                        if (style.Symbol.HighlightColor == Color.Transparent)
                        {
                            Color c2 = brush.BackColor;
                            c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                            brush = new BrushInfo(c2);
                        }
                        else
                        {
                            brush = new BrushInfo(style.Symbol.HighlightColor);
                        }
                    }
                    else if (rgn.IsChartPoint && rgn.SeriesIndex != serIndex)
                    {
                        if (style.Symbol.DimmedColor == Color.Transparent)
                        {
                            Color c2 = brush.BackColor;
                            c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                            brush = new BrushInfo(c2);
                            brush = new BrushInfo(50, brush);
                        }
                        else
                        {
                            brush = new BrushInfo(style.Symbol.DimmedColor);
                        }
                    }
                }
            }

            return brush;
        }
		/// <summary>
		/// Brush information is retrieved from the style associated with the index of the point to be rendered.
		/// It is then changed for special cases such as when automatic highlighting is enabled.
		/// </summary>
		/// <returns>Brush information that is to be used for filling elements displayed at this index.</returns>
		protected virtual BrushInfo GetBrush()
		{
			ChartStyleInfo style = SeriesStyle;
			BrushInfo brush = style.Interior;
			int serIndex = Chart.Series.IndexOf(m_series);

            if ((!Chart.AutoHighlight) && Chart.SeriesHighlight && (Chart.SeriesHighlightIndex != -1))
            {
                brush = this.GetSeriesHighlightBrush(serIndex, brush, style);           
            }
                                 
            if (Chart.AutoHighlight && Chart.ActiveIndex > 0)
			{
				int index = Chart.ActiveIndex;

				if (index >= 0 && index < this.Chart.ChartRegions.Count)
				{
					ChartRegion rgn = this.Chart.ChartRegions[index];

					if (rgn.SeriesIndex == serIndex)
					{
						if (style.HighlightInterior == null)
						{
							Color c2 = brush.BackColor;
							c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
							brush = new BrushInfo(c2);
						}
						else
						{
							brush = style.HighlightInterior;
						}                       
					}                    
				}             
			}
            else if (Chart.SeriesHighlight && Chart.ActiveIndex > 0) 
            {
                int index = Chart.ActiveIndex;

                if (index >= 0 && index < this.Chart.ChartRegions.Count)
                {
                    ChartRegion rgn = this.Chart.ChartRegions[index];

                    if (rgn.IsChartPoint && rgn.SeriesIndex == serIndex)
                    {
                        if (style.HighlightInterior == null)
                        {
                            Color c2 = brush.BackColor;
                            c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                            brush = new BrushInfo(c2);
                        }
                        else
                        {
                            brush = style.HighlightInterior;
                        }                                               
                    }
                    else if (rgn.IsChartPoint && rgn.SeriesIndex != serIndex)
                    {
                        if (style.DimmedInterior == null)
                        {
                            Color c2 = brush.BackColor;
                            c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                            brush = new BrushInfo(c2);
                            brush = new BrushInfo(50, brush);
                        }
                        else
                        {
                            brush = style.DimmedInterior;
                        }
                    }
                }
            }   
            
			return brush;
		}

        /// <summary>
        /// Brush information is retrieved from the style associated with the index of the point to be rendered when SeriesHighlight is enabled.
        /// It is then changed for special cases such as when automatic highlighting is enabled.
        /// </summary>
        /// <returns>Brush information that is to be used for filling elements displayed at this index.</returns>
        /// <param name="serIndex">Specfies the series index.</param>
        /// <param name="brush">Interior of the specified series..</param>
        /// <param name="style">Series .</param>
        internal BrushInfo GetSeriesHighlightBrush(int serIndex, BrushInfo brush, ChartStyleInfo style )
        {   
                              
            if (serIndex == Chart.SeriesHighlightIndex)
            {
                if (style.HighlightInterior == null)
                {
                    Color c2 = brush.BackColor;
                    c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                    brush = new BrushInfo(c2);
                }
                else
                {
                    brush = style.HighlightInterior;
                }
            }
            else
            {
                if (style.DimmedInterior == null)
                {
                    Color c2 = brush.BackColor;
                    c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                    brush = new BrushInfo(c2);
                    brush = new BrushInfo(50, brush);
                }
                else
                {
                    brush = style.DimmedInterior;
                }
            }           

            return brush;
        }

        /// <summary>
        /// Gets the symbol highlight brush.
        /// </summary>
        /// <param name="serIndex">Index of the ser.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        internal BrushInfo GetSymbolHighlightBrush(int serIndex, BrushInfo brush, ChartStyleInfo style)
        {
            if (serIndex == Chart.SeriesHighlightIndex)
            {
                if (style.Symbol.HighlightColor == Color.Transparent)
                {
                    Color c2 = brush.BackColor;
                    c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                    brush = new BrushInfo(c2);
                }
                else
                {
                    brush = new BrushInfo(style.Symbol.HighlightColor);
                }
            }
            else
            {
                if (style.Symbol.DimmedColor == Color.Transparent)
                {
                    Color c2 = brush.BackColor;
                    c2 = Color.FromArgb(255 - c2.R, 255 - c2.G, 255 - c2.B);
                    brush = new BrushInfo(c2);
                    brush = new BrushInfo(50, brush);
                }
                else
                {
                    brush = new BrushInfo(style.Symbol.DimmedColor);
                }
            }

            return brush;
        }

		/// <summary>
		/// Gets the phong interior.
		/// </summary>
		/// <param name="brushInfo">The base brush info.</param>
		/// <param name="lightColor">Color of the light.</param>
		/// <param name="lightAlpha">The light alpha.</param>
		/// <param name="phongAlpha">The phong alpha.</param>
		/// <returns></returns>
		protected BrushInfo GetPhongInterior(BrushInfo brushInfo,
			Color lightColor, double lightAlpha, double phongAlpha)
		{
			if (brushInfo.Style == BrushStyle.Solid)
			{
				float[] pos;
				Color[] col;

				GradientStyle style = this.IsInvertedAxes ? GradientStyle.Vertical : GradientStyle.Horizontal;
				ChartSeriesRenderer.PhongShadingColors(brushInfo.BackColor, brushInfo.BackColor,
					lightColor, lightAlpha, phongAlpha, out col, out pos);

				brushInfo = new BrushInfo(style, col);
			}

			return brushInfo;
		}
		#endregion

		#region Computes the coordinates, stacing info, etc.
		/// <summary>
		/// Calculates the point that is considered to be the low anchor point of a series. This
		/// value is used when rendering text below chart point elements.
		/// </summary>
		/// <param name="index">Index value of the point for which the value is requested.</param>
		/// <returns>Calculated value that is to be used as the base anchor point.</returns>
		protected virtual float GetLowerAnchorPointValue(int index)
		{
			if (m_series.BaseStackingType != ChartSeriesBaseStackingType.NotStacked)
			{
				return GetStackInfo(index);
			}

			if (m_series.RequireInvertedAxes)
			{
				return this.CustomOriginX;
			}

			return CustomOriginY;
		}

		/// <summary>
		/// Overloaded. Given a point index, returns the point to be plotted on the chart.
		/// </summary>
		/// <param name="i">X Index value</param>
		/// <param name="j">Y Index value</param>
		/// <returns>Point to be plotted</returns>
		protected PointF GetPointFromIndex(int i, int j)
		{
			return new PointF(GetXFromIndex(i, j), GetYFromIndex(i, j));
		}

		/// <summary>
		/// Given a point index, returns the point to be plotted on the chart.
		/// </summary>
		/// <param name="i">X index. Y Index is taken as 0.</param>
		/// <returns>Point to be plotted.</returns>
		public PointF GetPointFromIndex(int i)
		{
			return GetPointFromIndex(i, 0);
		}

		/// <summary>
		/// Compute real point from specified <see cref="ChartStyledPoint"/>
		/// </summary>
		/// <param name="cpt">The <see cref="ChartStyledPoint"/>.</param>
		/// <returns></returns>
		protected virtual PointF GetPointFromValue(ChartStyledPoint cpt)
		{
			return this.GetPointFromValue(cpt.X, cpt.YValues[0]);
		}

		/// <summary>
		/// Compute real point from specified coordinates.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		protected virtual PointF GetPointFromValue(double x, double y)
		{
			if (this.IsRadial)
			{
				PointF center = ChartMath.GetCenter(this.Bounds);

				double angle = ChartMath.HlfPI;

				if (m_series.Type == ChartSeriesType.Radar)
				{				
                    if (m_series.ActualXAxis.Inversed)
                        angle += ChartMath.DblPI * (m_series.ActualXAxis.Range.Max - x) / m_series.ActualXAxis.Range.Delta;
                    else
                        angle += ChartMath.DblPI * (x- m_series.ActualXAxis.Range.Min) / m_series.ActualXAxis.Range.Delta;
				}
				else if (m_series.Type == ChartSeriesType.Polar)
				{                  
                    if (m_series.ActualXAxis.RangeType == ChartAxisRangeType.Set)
                    {                      
                        if (m_series.ActualXAxis.Inversed)
                            angle += ChartMath.DblPI * (m_series.ActualXAxis.Range.Max - x) / m_series.ActualXAxis.Range.Delta;
                        else
                            angle += ChartMath.DblPI * (x - m_series.ActualXAxis.Range.Min) / m_series.ActualXAxis.Range.Delta;
                    }
                    else
                    {                       
                        if (m_series.ActualXAxis.Inversed)
                            angle += (ChartMath.DblPI - x);
                        else
                            angle += x;
                    }
				}

				float radius = m_series.ActualYAxis.GetVisibleValue(y);

				return new PointF(center.X + (float)(radius * Math.Cos(angle)),
					center.Y - (float)(radius * Math.Sin(angle)));
			}

			if (this.IsInvertedAxes)
			{
				return new PointF(XAxis.GetCoordinateFromValue(y), YAxis.GetCoordinateFromValue(x));
			}

			return new PointF(XAxis.GetCoordinateFromValue(x), YAxis.GetCoordinateFromValue(y));
		}

		/// <summary>
		/// Gets the point from value.
		/// </summary>
		/// <param name="cpt">The CPT.</param>
		/// <returns></returns>
        protected PointF GetPointFromValue(ChartPoint cpt)
		{
			return this.GetPointFromValue(cpt, 0);
		}
        

		/// <summary>
		/// Gets the point from value.
		/// </summary>
		/// <param name="cpt">The <see cref="ChartPoint"/>.</param>
		/// <param name="j">The index of Y value.</param>
		/// <returns></returns>
		protected PointF GetPointFromValue(ChartPoint cpt, int j)
		{
			return new PointF(GetXFromValue(cpt, j), GetYFromValue(cpt, j));
		}

		/// <summary>
		/// Gets the side by side range.
		/// </summary>
		/// <returns></returns>
		protected virtual DoubleRange GetSideBySideRange()
		{
			double width = 1 - 0.01 * m_chart.Spacing;
			double minWidth = this.GetMinPointsDelta();

			int pos = -1;
			int all = 0;

			if (m_series.BaseType == ChartSeriesBaseType.SideBySide)
			{
				Hashtable stackedTypes = new Hashtable();

				foreach (ChartSeries ser in m_chart.Series)
				{
					if (ser.Visible && m_series.BaseType == ChartSeriesBaseType.SideBySide)
					{
						if (ser.BaseStackingType != ChartSeriesBaseStackingType.NotStacked)
						{
							if (!stackedTypes.ContainsKey(ser.Type))
							{
								all++;
								stackedTypes.Add(ser.Type, all);
							}

							if (m_series == ser)
							{
								pos = (int)stackedTypes[ser.Type];
							}
						}
						else
						{
							all++;

							if (m_series == ser)
							{
								pos = all;
							}
						}
					}
				}
			}

			if (all == 0)
			{
				all = 1;
				pos = 0;
			}

			minWidth = (minWidth == double.MaxValue) ? 1d : minWidth;

			double div = minWidth * width / all;
			double start = div * (pos - 1) - minWidth * width / 2;
			double end = start + div;

			return new DoubleRange(start, end);
		}

		/// <summary>
		/// Gets the "side by side" info.
		/// </summary>
		/// <returns></returns>
		protected DoubleRange GetSideBySideInfo()
		{
			return m_series.ChartModel.GetSideBySideInfo(this.ChartArea, m_series);
		}

		/// <summary>
		/// Overloaded. This method is used when series are rendered as stacked data. The value returned is a cumulative value of
		/// Y from all series that are below the series currently being rendered.
		/// </summary>
		/// <param name="i"></param>
		/// <returns>Value that gives the position from which this series should be rendered.</returns>
		protected virtual float GetStackInfo(int i)
		{
			double y = this.GetStackInfoValue(i);

			return this.IsInvertedAxes ? this.GetXFromCoordinate(y) : this.GetYFromCoordinate(y);
		}

		/// <summary>
		/// Overloaded. This method is used when series are rendered as stacked data. The value returned is a cumulative value of
		/// Y from all series that are below the series currently being rendered.
		/// </summary>
		/// <param name="i">The index of point.</param>
		/// <returns>Value that gives the position from which this series should be rendered.</returns>
		protected virtual double GetStackInfoValue(int i)
		{
			return m_series.ChartModel.GetStackInfo(this.ChartArea, m_series, i, false);
		}

		/// <summary>
		/// Overloaded. This method is used when series are rendered as stacked data. The value returned is a cumulative value of
		/// Y from all series that are below the series currently being rendered.
		/// </summary>
		/// <param name="i">The index of point.</param>
		/// <param name="isWithMe">if set to <c>true</c> the Y value of point will be added to result.</param>
		/// <returns>Value that gives the position from which this series should be rendered.</returns>
		protected virtual double GetStackInfoValue(int i, bool isWithMe)
		{
			return m_series.ChartModel.GetStackInfo(this.ChartArea, m_series, i, isWithMe);
		}

		/// <summary>
		/// Returns the anchor point at which the symbol associated with an index is to be
		/// displayed.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <returns></returns>
		protected PointF GetSymbolPoint(ChartStyledPoint point)
		{
			PointF pt = this.GetSymbolCoordinates(point);

			if (m_series.BaseType != ChartSeriesBaseType.Circular)
			{
				pt = PointF.Add(pt, this.GetThisOffset());
			}

			return pt;
		}

		/// <summary>
		/// Gets the symbol vector.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <returns></returns>
		protected Vector3D GetSymbolVector(ChartStyledPoint point)
		{
			PointF pt = this.GetSymbolCoordinates(point);

			return new Vector3D(pt.X, pt.Y, this.GetPlaceDepth());
		}

		/// <summary>
		/// Gets the symbol coordinates.
		/// </summary>
		/// <param name="styledPoint">The styled point.</param>
		/// <returns></returns>
		protected virtual PointF GetSymbolCoordinates(ChartStyledPoint styledPoint)
		{
			PointF pt;

			double x = styledPoint.X;
			double y = styledPoint.YValues[0];

			if (m_series.BaseType == ChartSeriesBaseType.SideBySide)
			{
				x += this.GetSideBySideInfo().Median;
			}

			if (m_series.BaseStackingType == ChartSeriesBaseStackingType.Stacked)
			{
				y += this.GetStackInfoValue(styledPoint.Index);
			}
			else if (m_series.BaseStackingType == ChartSeriesBaseStackingType.FullStacked)
			{
				y = this.GetStackInfoValue(styledPoint.Index, true);
			}

			if (m_series.Type == ChartSeriesType.Tornado && styledPoint.YValues.Length > 1)
			{
				pt = this.GetPointFromValue(x, styledPoint.YValues[1]);
			}
			else
			{
				pt = this.GetPointFromValue(x, y);
			}

			Size symbolOffset = styledPoint.Style.Symbol.Offset;

			pt.X += symbolOffset.Width;
			pt.Y += symbolOffset.Height;

            RectangleF ClipBounds = this.Chart.GetGraphics().ClipBounds;
            pt.X = (Math.Abs(pt.X) > ClipBounds.Width ? ClipBounds.Width : pt.X);
            pt.Y = (Math.Abs(pt.Y) > ClipBounds.Width ? ClipBounds.Width : pt.Y);

			return pt;
		}

		///// <summary>
		///// Returns the anchor point at which the symbol associated with an index is to be
		///// displayed.
		///// </summary>
		///// <param name="index">Index value of the point for which the value is requested.</param>
		///// <returns></returns>
		//[Obsolete]
		//protected virtual PointF GetSymbolPoint(int index)
		//{
		//  PointF pt;
		//  ChartPoint cp = new ChartPoint(m_series.Points[index].X,
		//    m_series.Points[index].YValues.Clone() as double[]);

		//  if (m_series.BaseType == ChartSeriesBaseType.SideBySide)
		//  {
		//    cp.X += this.GetSideBySideInfo().Median;
		//  }

		//  if (m_series.BaseStackingType == ChartSeriesBaseStackingType.Stacked)
		//  {
		//    cp.YValues[0] += this.GetStackInfoValue(index);
		//  }
		//  else if (m_series.BaseStackingType == ChartSeriesBaseStackingType.FullStacked)
		//  {
		//    cp.YValues[0] = this.GetStackInfoValue(index, true);
		//  }

		//  if (m_series.Type == ChartSeriesType.Tornado && cp.YValues.Length > 1)
		//  {
		//    pt = this.GetPointFromValue(cp, 1);
		//  }
		//  else
		//  {
		//    pt = this.GetPointFromValue(cp, 0);
		//  }

		//  if (m_series.BaseType != ChartSeriesBaseType.Circular)
		//  {
		//    SizeF sz = this.GetThisOffset();

		//    pt.X += sz.Width;
		//    pt.Y += sz.Height;
		//  }

		//  Size symbolOffset = this.GetStyleAt(index).Symbol.Offset;

		//  pt.X += symbolOffset.Width;
		//  pt.Y += symbolOffset.Height;

		//  return pt;
		//}
		///// <summary>
		///// Returns the anchor vector at which the symbol associated with an index is to be
		///// displayed.
		///// </summary>
		///// <param name="index">Index value of the point for which the value is requested.</param>
		///// <returns></returns>
		//[Obsolete]
		//protected virtual Vector3D GetSymbolVector(int index)
		//{
		//  PointF pt;
		//  float sd = Place * ChartArea.Depth / PlaceSize;
		//  float z = 0;

		//  if (m_series.Type == ChartSeriesType.Tornado)
		//  {
		//    if (m_series.Points[index].YValues.Length > 1)
		//      pt = this.GetPointFromIndex(index, 1);
		//    else pt = this.GetPointFromIndex(index, 0);
		//  }
		//  else
		//  {

		//    ChartPoint cp = m_series.Points[index];
		//    double sideBySideOffset = 0.0;
		//    double stackingOffset = 0.0;

		//    if (m_series.BaseType == ChartSeriesBaseType.SideBySide)
		//    {
		//      sideBySideOffset = this.GetSideBySideInfo().Median;
		//    }

		//    if (m_series.BaseStackingType == ChartSeriesBaseStackingType.Stacked)
		//    {
		//      stackingOffset = this.GetStackInfoValue(index);
		//    }

		//    ChartPoint truCP = new ChartPoint(cp.X + sideBySideOffset, cp.YValues[0] + stackingOffset);

		//    if (m_series.BaseStackingType == ChartSeriesBaseStackingType.FullStacked)
		//    {
		//      truCP.YValues[0] = this.GetStackInfoValue(index, true);
		//    }

		//    pt = new PointF(GetXFromValue(truCP, 0), GetYFromValue(truCP, 0));
		//  }
		//  z = sd;

		//  return new Vector3D(pt.X, pt.Y, this.GetPlaceDepth());
		//}

		/// <summary>
		/// Given a point index, returns the X value to be plotted on the chart.
		/// </summary>
		/// <param name="i">X index</param>
		/// <param name="j">Y index</param>
		/// <returns>X value to be plotted.</returns>
		protected virtual float GetXFromIndex(int i, int j)
		{
			return this.GetXFromValue(m_series.Points[i], j);
		}

		/// <summary>
		/// Given a point and y value, returns the X value to be plotted on the chart.
		/// </summary>
		/// <param name="cp">The chart point</param>
		/// <param name="j">The Y index</param>
		/// <returns>X value to be plotted.</returns>
		protected virtual float GetXFromValue(ChartPoint cp, int j)
		{
			float res = 0;

			if (m_series.BaseType == ChartSeriesBaseType.Circular)
			{
				PointF center = ChartArea.Center;
				float radius = m_series.ActualYAxis.GetVisibleValue(cp.YValues[j]);

				if (cp.IsEmpty)
				{
					res = center.X;
				}
				else
				{
					double angle = GetAngleValue(cp, m_series);
                    return (float)(center.X + radius * Math.Cos(angle));                   					
				}
			}

			return this.GetXFromCoordinate(this.GetXAxisValue(cp, j));
		}

		/// <summary>
		/// Given an X coordinate value, returns the display value.
		/// </summary>
		/// <param name="value">Coordinate on the axis.</param>
		/// <returns>Display value.</returns>
		protected virtual float GetXFromCoordinate(double value)
		{
			return XAxis.GetCoordinateFromValue(value);
		}

		/// <summary>
		/// Given the point indices, returns the Y value to be plotted on the chart.
		/// </summary>
		/// <param name="i">X index</param>
		/// <param name="j">Y index</param>
		/// <returns>Y value to be plotted</returns>
		protected virtual float GetYFromIndex(int i, int j)
		{
			float f = 0;

			if (m_series.Type == ChartSeriesType.Radar || m_series.Type == ChartSeriesType.Polar)
			{
				float radius = m_series.ActualYAxis.GetVisibleValue(m_series.Points[i].YValues[j]);

				PointF center = ChartArea.Center;

				if (m_series.Points[i].IsEmpty)
				{
					return center.Y;
				}

				double angle = GetAngleValue(i, m_series.Points[i], m_series);

				return (float)(center.Y - radius * Math.Sin(angle));
			}

			bool inverted = (m_series.RequireInvertedAxes || (this.IgnoreSeriesInversion && m_chart.RequireInvertedAxes));

			if (inverted)
			{
				f = this.GetYFromCoordinate(m_series.Points[i].X);

				if (m_chart.Indexed)
				{
					f = this.GetYFromCoordinate(this.GetIndexValueFromX(m_series.Points[i].X));
				}

				if (m_series.XType == ChartValueType.Custom)
				{
					f = this.GetYFromCoordinate(i);
				}
			}
			else
			{
				f = this.GetYFromCoordinate(m_series.Points[i].YValues[j]);
			}

			return f;
		}

		/// <summary>
		/// Given a point and y value, returns the Y value to be plotted on the chart.
		/// </summary>
		/// <param name="cp">The chart point value.</param>
		/// <param name="j">The Y index</param>
		/// <returns>Y value to be plotted</returns>
		protected virtual float GetYFromValue(ChartPoint cp, int j)
		{
			if (this.IsRadial)
			{
				PointF center = ChartArea.Center;

				if (cp.IsEmpty)
				{
					return center.Y;
				}

				double angle = this.GetAngleValue(cp, m_series);
				float radius = m_series.ActualYAxis.GetVisibleValue(cp.YValues[j]);

				return center.Y - (float)(radius * Math.Sin(angle));
			}

			return this.GetYFromCoordinate(this.GetYAxisValue(cp, j));
		}

		/// <summary>
		/// Given a Y coordinate value, returns the display value.
		/// </summary>
		/// <param name="y"></param>
		/// <returns>Display value.</returns>
		protected virtual float GetYFromCoordinate(double y)
		{
			return YAxis.GetCoordinateFromValue(y);
		}

		/// <summary>
		/// This function transforms x vales of series points to index vales.
		/// Also it populates index hash table.
		/// </summary>
		/// <param name="x">The X value of <see cref="ChartPoint"/>.</param>
		/// <returns></returns>
		protected virtual double GetIndexValueFromX(double x)
		{
			if (m_chart != null && m_chart.Indexed)
			{
				return m_chart.IndexValues.GetIndex(x);
			}

			return x;
		}

		/// <summary>
		/// Gets the angle value.
		/// </summary>
		/// <param name="cp">The cp.</param>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		protected virtual double GetAngleValue(ChartPoint cp, ChartSeries series)
		{
			double result = 0;

			if (series.Type == ChartSeriesType.Radar)
			{               
                if (series.ActualXAxis.Inversed)
                    result = ChartMath.DblPI * (series.ActualXAxis.Range.Max - cp.X) / series.ActualXAxis.Range.Delta;
                else
                    result = ChartMath.DblPI * (cp.X - series.ActualXAxis.Range.Min) / series.ActualXAxis.Range.Delta;
			}
			else if (series.Type == ChartSeriesType.Polar)
			{
                if (series.ActualXAxis.RangeType == ChartAxisRangeType.Set)
                {                  
                    if (series.ActualXAxis.Inversed)
                        result = ChartMath.DblPI * (series.ActualXAxis.Range.Max - cp.X) / series.ActualXAxis.Range.Delta;
                    else
                        result = ChartMath.DblPI * (cp.X - series.ActualXAxis.Range.Min) / series.ActualXAxis.Range.Delta;
                }
                else
                {                    
                    if (series.ActualXAxis.Inversed)
                        result = (ChartMath.DblPI - cp.X);
                    else
                        result = cp.X;
                }
			}       
           
			return result + ChartMath.HlfPI;
		}

		/// <summary>
		/// Gets the angle by X value.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="cp">The cp.</param>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		protected virtual double GetAngleValue(int index, ChartPoint cp, ChartSeries series)
		{
			double result = Math.PI / 2;

			if (m_chart.Indexed)
			{
				if (m_series.Type == ChartSeriesType.Radar)
				{
					result += index * ChartMath.DblPI / m_chart.IndexValues.Count;                  
				}
				else if (m_series.Type == ChartSeriesType.Polar)
				{
                    if (m_series.ActualXAxis.RangeType == ChartAxisRangeType.Set) 
                    {
                        result += index * ChartMath.DblPI / m_chart.IndexValues.Count;  
                    }
                    else
                    {
                        result += index;
                    }
				}
			}
			else
			{
				result = GetAngleValue(cp, series);
			}
            
			return result;
		}

		/// <summary>
		/// Returns the value form <see cref="ChartPoint"/> requiring by X axis.
		/// </summary>
		/// <param name="cpt">Instance of <see cref="ChartPoint"/></param>
		/// <param name="j">Index of Y value from specified point.</param>
		/// <returns>Require value for axis.</returns>
		private double GetXAxisValue(ChartPoint cpt, int j)
		{
			return this.IsInvertedAxes ? cpt.YValues[j] : this.GetIndexValueFromX(cpt.X);
		}

		/// <summary>
		/// Returns the value form <see cref="ChartPoint"/> requiring by Y axis.
		/// </summary>
		/// <param name="cpt">Instance of <see cref="ChartPoint"/></param>
		/// <param name="j">Index of Y value from specified point.</param>
		/// <returns>Require value for axis.</returns>
		private double GetYAxisValue(ChartPoint cpt, int j)
		{
			return this.IsInvertedAxes ? this.GetIndexValueFromX(cpt.X) : cpt.YValues[j];
		}

		/// <summary>
		/// Measures the X range.
		/// </summary>
		/// <returns></returns>
		public virtual DoubleRange GetXDataMeasure()
		{
			if (m_series.Points.Count > 0)
			{
				double max = double.MinValue;
				double min = double.MaxValue;

				for (int j = 0; j < m_series.Points.Count; j++)
				{
					double x = m_series.Points[j].X;

					if (x > max)
					{
						max = x;
					}

					if (x < min)
					{
						min = x;
					}
				}

				return new DoubleRange(min, max);
			}

			return DoubleRange.Empty;
		}

		/// <summary>
		/// Measures the X range.
		/// </summary>
		/// <returns></returns>
		public virtual DoubleRange GetYDataMeasure()
		{
			if (m_series.Points.Count > 0)
			{
				double max = double.MinValue;
				double min = double.MaxValue;

				for (int i = 0; i < m_series.Points.Count; i++)
				{
					double[] yValues = m_series.Points[i].YValues;

					for (int j = 0, cj = Math.Min(yValues.Length, this.RequireYValuesCount); j < cj; j++)
					{
						if (yValues[j] > max)
						{
							max = yValues[j];
						}

						if (yValues[j] < min)
						{
							min = yValues[j];
						}
					}
				}

				DoubleRange range = new DoubleRange(min, max);

				if (m_series.OriginDependent)
				{
					range += m_series.ActualYAxis.CurrentOrigin;
				}

				return range;
			}

			return DoubleRange.Empty;
		}
		#endregion

		#region Draw Adornments
		/// <summary>
		/// Overloaded. Renders text. Performs positioning and delegates to the <see cref="RenderingHelper"/> class.
		/// </summary>
		/// <param name="g">Renders text. Performs positioning and delegates to the <see cref="RenderingHelper"/> class.</param>
		/// <param name="styledPoint">The associated point.</param>
		/// <param name="p">Point that is to be used as the anchor.</param>
		protected virtual void DrawText(Graphics g, ChartStyledPoint styledPoint, PointF p)
		{
			ChartStyleInfo style = styledPoint.Style;
			int index = styledPoint.Index;
			PointF offset = new PointF(0.0f, 0.0f);
			PointF pt = new PointF(p.X, p.Y);
            
			SizeF sz = g.MeasureString(style.Text, style.GdipFont);
			SizeF siz = new SizeF(sz.Width / 2 + style.TextOffset, sz.Height / 2 + style.TextOffset);

			#region Orientation cases
			switch (style.TextOrientation)
			{
				case ChartTextOrientation.Up:
					pt.Y -= siz.Height;
					break;

				case ChartTextOrientation.Down:
					pt.Y += siz.Height;
					break;

				case ChartTextOrientation.Left:
					pt.X -= siz.Width;
					break;

				case ChartTextOrientation.Right:
					pt.X += siz.Width;
					break;

				case ChartTextOrientation.UpLeft:
					pt.X -= siz.Width;
					pt.Y -= siz.Height;
					break;

				case ChartTextOrientation.DownLeft:
					pt.X -= siz.Width;
					pt.Y += siz.Height;
					break;

				case ChartTextOrientation.UpRight:
					pt.X += siz.Width;
					pt.Y -= siz.Height;
					break;

				case ChartTextOrientation.DownRight:
					pt.X += siz.Width;
					pt.Y += siz.Height;
					break;

				#region other cases
				case ChartTextOrientation.Smart:
					{
						if (m_chart.RequireInvertedAxes)
						{
							if (pt.X < this.CustomOriginX)
							{
								pt.X += -siz.Width;
							}

							else
							{
								pt.X += siz.Width;
							}
						}
						else
						{
							if (pt.Y < this.CustomOriginY)
							{
								pt.Y += -siz.Height;
							}
							else
							{
								pt.Y += siz.Height;
							}
						}
						break;
					}

				case ChartTextOrientation.RegionUp:
					{
						if (m_series.RequireInvertedAxes)
						{
							pt.X -= sz.Width;
						}
						else
						{
							pt.Y += sz.Height;
						}
						break;
					}

				case ChartTextOrientation.RegionDown:
					{
						float f = GetLowerAnchorPointValue(index);

						if (m_series.RequireInvertedAxes)
						{
							pt.X = f;
						}

						else
						{
							pt.Y = f - sz.Height;
						}
						break;
					}

				case ChartTextOrientation.RegionCenter:
					{
						float f = GetLowerAnchorPointValue(index);

						if (m_series.RequireInvertedAxes)
						{
							pt.X = (f + pt.X) / 2;
						}

						else
						{
							pt.Y = (f + pt.Y) / 2;
						}
						break;
					}

				case ChartTextOrientation.SymbolCenter:
					{
						pt = this.GetSymbolPoint(styledPoint);
						break;
					}
				#endregion
			}
			#endregion

			Rectangle car = ChartArea.RenderBounds;
			double angle = ChartMath.ToRadians * style.Font.Orientation;
			float sin = (float)Math.Sin(angle);
			float cos = (float)Math.Cos(angle);

			if (m_series.SmartLabels)
			{
				sz = g.MeasureString(style.Text, style.Font.GdipFont).ToSize();
				sz.Width += 1;

				pt.X += -sz.Width / 2 + offset.X;
				pt.Y += -sz.Height / 2 + offset.Y;

				RectangleF startrrect = new RectangleF(pt, sz);
				RectangleF rrect = ChartMath.LeftCenterRotatedRectangleBounds(startrrect, angle);
				ChartLabel text_label = new ChartLabel(rrect.Location, p, rrect.Size, SizeF.Empty);
				RectangleF layouted_rrect = m_labelLayoutManager.AddLabel(text_label);
				RectangleF rrect_back = rrect;
				PointF layout_location = new PointF(startrrect.X + layouted_rrect.X - rrect.X,
																						 startrrect.Y + layouted_rrect.Y - rrect.Y);
				rrect_back = new RectangleF(layout_location, startrrect.Size);
				pt = rrect_back.Location;
				if (angle != 0)
				{
					pt.X += -sz.Width / 2;
				}
                text_label.DrawPointingLine(g, style, m_series);
			}
			else
			{
				if (angle != 0)
				{
					float pT = car.Top - pt.Y;
					if (pT > 0 && Math.Abs(pT) < sz.Width)// here we check whether drawing (and rotation) origin didn't pass the bounds.
					{
						pt.X = car.Top;
						pT = 0;
					}
					float pB = car.Bottom - pt.Y;
					if (pB < 0 && Math.Abs(pB) < sz.Width)// here we check whether drawing (and rotation) origin didn't pass the bounds.
					{
						pt.X = car.Bottom;
						pB = 0;
						pT = -car.Height;
					}
					float pL = car.Left - pt.X;
					if (pL > 0 && Math.Abs(pL) < sz.Width)// here we check whether drawing (and rotation) origin didn't pass the bounds.
					{
						pt.X = car.Left;
						pL = 0;
					}
					float pR = car.Right - pt.X;
					if (pR < 0 && Math.Abs(pR) < sz.Width)// here we check whether drawing (and rotation) origin didn't pass the bounds.
					{
						pt.X = car.Right;
						pR = 0;
						pL = -car.Width;
					}


					float wT = pT / (float)Math.Sin(angle);
					if (wT <= 0.0f) wT = float.MaxValue;

					float wB = pB / (float)Math.Sin(angle);
					if (wB <= 0.0f) wB = float.MaxValue;

					float wL = pL / (float)Math.Cos(angle);
					if (wL <= 0.0f) wL = float.MaxValue;

					float wR = pR / (float)Math.Cos(angle);
					if (wR <= 0.0f) wR = float.MaxValue;

					float minWidth = Math.Min(wT, wB);
					minWidth = Math.Min(wL, minWidth);
					minWidth = Math.Min(wR, minWidth);

                    if (style.Format != null)
                    {
                        sz = g.MeasureString(style.Text, style.GdipFont);
                    }
                    else
                    {
                        sz = g.MeasureString(style.Text, style.GdipFont, (int)Math.Round(minWidth - style.GdipFont.Height / 2));
                    }
					sz.Width += 1;
				}

				pt.Y += -sz.Height / 2 + offset.Y;

				if (angle == 0)
				{
					pt.X += -sz.Width / 2 + offset.X;
					float pT = car.Top - pt.Y;
					float pB = car.Bottom - pt.Y;
					float pL = car.Left - pt.X;
					float pR = car.Right - (pt.X + sz.Width);

					if (pT > 0 && Math.Abs(pT) < sz.Height)
						pt.Y = car.Top;
					if (pB < 0 && Math.Abs(pB) < sz.Height)
						pt.Y = car.Bottom - sz.Height;
					if (pL > 0 && Math.Abs(pL) < sz.Width)
					{
						pt.X = car.Left;
                        if (style.Format != null)
                        {
                            sz = g.MeasureString(style.Text, style.GdipFont);
                        }
                        else
                        {
                            sz = g.MeasureString(style.Text, style.GdipFont, (int)Math.Round(sz.Width - pL));
                        }
					}
					if ((pR < 0) && (Math.Abs(pR) < sz.Width))
					{
                        if (style.Format != null)
                        {
                            sz = g.MeasureString(style.Text, style.GdipFont);
                        }
                        else
                        {
                            sz = g.MeasureString(style.Text, style.GdipFont, (int)Math.Round(sz.Width + pR));
                        }
					}
					sz.Width += 1;
				}
			}
			if (!pt.IsEmpty)
			{
                //Draw Shape around the display text of series
			    if (style.DrawTextShape)
			    {
                    Rectangle bounds = new Rectangle((int)pt.X, (int)pt.Y, (int)sz.Width, (int)sz.Height);
                    
                    #region Selct shape Type to draw around the series display text
                    switch (style.TextShape.Type)
                     {
                         case ChartCustomShape.Circle:
                             using (SolidBrush brush = new SolidBrush(style.TextShape.Color))
                             {
                                 bounds = new Rectangle((int)pt.X - 2, (int)pt.Y - 2, (int)sz.Width + 5, (int)sz.Height + 5);
                                 g.DrawEllipse(style.TextShape.Border.GdipPen, bounds);
                                 g.FillEllipse(brush, bounds);
                             }
                             break;                        

                         case ChartCustomShape.Pentagon:
                             bounds = new Rectangle((int)pt.X - 5, (int)pt.Y - 5, (int)sz.Width + 10, (int)sz.Height + 10);
                             using (SolidBrush brush = new SolidBrush(style.TextShape.Color))
                             {
                                 PointF point1 = new PointF(bounds.X + (float)bounds.Width / 5f, bounds.Y);
                                 PointF point2 = new PointF(bounds.X + bounds.Width - (float)bounds.Width / 5f, bounds.Y);
                                 PointF point3 = new PointF(bounds.Right, bounds.Y + (float)bounds.Height * (3f / 5f));
                                 PointF point4 = new PointF(bounds.X + (float)bounds.Width / 2f, bounds.Bottom);
                                 PointF point5 = new PointF(bounds.X, bounds.Y + (float)bounds.Height * (3f / 5f));

                                 PointF[] curvePoints = { point1, point2, point3, point4, point5 };

                                 g.DrawPolygon(style.TextShape.Border.GdipPen, curvePoints);
                                 g.FillPolygon(brush, curvePoints);
                             }
                             break;

                         case ChartCustomShape.Hexagon:
                             bounds = new Rectangle((int)pt.X - 5, (int)pt.Y - 5, (int)sz.Width + 10, (int)sz.Height + 10);
                             using (SolidBrush brush = new SolidBrush(style.TextShape.Color))
                             {
                                 PointF point1 = new PointF(bounds.X + bounds.Width / 4, bounds.Y);
                                 PointF point2 = new PointF(bounds.X + bounds.Width * (3f / 4f), bounds.Y);
                                 PointF point3 = new PointF(bounds.Right, bounds.Y + bounds.Height / 2);
                                 PointF point4 = new PointF(bounds.X + bounds.Width * (3f / 4f), bounds.Bottom);
                                 PointF point5 = new PointF(bounds.X + bounds.Width / 4, bounds.Bottom);
                                 PointF point6 = new PointF(bounds.X, bounds.Y + bounds.Height / 2);

                                 PointF[] curvePoints = { point1, point2, point3, point4, point5, point6 };

                                 g.DrawPolygon(style.TextShape.Border.GdipPen, curvePoints);
                                 g.FillPolygon(brush, curvePoints);
                             }
                             break;

                         case ChartCustomShape.Square:
                         default:
                             using (SolidBrush brush = new SolidBrush(style.TextShape.Color))
                             {
                                 g.DrawRectangle(style.TextShape.Border.GdipPen, bounds);
                                 g.FillRectangle(brush, bounds);
                             }
                             break;
                     }
                    #endregion 
                }

                // the center of the string would be in the pt point if angle = 0,
                // otherwise, point pt indicates the beginning of the string.
                RenderingHelper.DrawText(g, style, pt, sz);
			}
		}
		/// <summary>
		/// Renders text. Performs positioning and delegates to the <see cref="RenderingHelper"/> class.
		/// </summary>
		/// <param name="g">The graphics object that is to be used.</param>
		/// <param name="styledPoint">The associated point.</param>
		/// <param name="p">The point that is to be used as anchor.</param>
		/// <param name="sz">The display size of the string.</param>
		protected virtual void DrawText(Graphics3D g, ChartStyledPoint styledPoint, Vector3D p, Size sz)
		{
			ChartStyleInfo style = styledPoint.Style;
			int index = styledPoint.Index;
			PointF offset = new PointF(0.0f, 0.0f);
			PointF pt = new PointF((float)p.X, (float)p.Y);
			PointF po = new PointF((float)p.X, (float)p.Y);

			SizeF siz = new SizeF(sz.Width / 2 + style.TextOffset, sz.Height / 2 + style.TextOffset);

			#region Orientation cases
			switch (style.TextOrientation)
			{

				case ChartTextOrientation.Up:
					pt.Y -= siz.Height;
					break;

				case ChartTextOrientation.Down:
					pt.Y += siz.Height;
					break;

				case ChartTextOrientation.Left:
					pt.X -= siz.Width;
					break;

				case ChartTextOrientation.Right:
					pt.X += siz.Width;
					break;

				case ChartTextOrientation.UpLeft:
					pt.X -= siz.Width;
					pt.Y -= siz.Height;
					break;

				case ChartTextOrientation.DownLeft:
					pt.X -= siz.Width;
					pt.Y += siz.Height;
					break;

				case ChartTextOrientation.UpRight:
					pt.X += siz.Width;
					pt.Y -= siz.Height;
					break;

				case ChartTextOrientation.DownRight:
					pt.X += siz.Width;
					pt.Y += siz.Height;
					break;


				case ChartTextOrientation.Smart:
					{

						if (m_chart.RequireInvertedAxes)
						{

							if (pt.X < this.CustomOriginX)
							{
								pt.X += -siz.Width;
							}

							else
							{
								pt.X += siz.Width;
							}
						}

						else
						{

							if (pt.Y < this.CustomOriginY)
							{
								pt.Y += -siz.Height;
							}

							else
							{
								pt.Y += siz.Height;
							}
						}
						break;
					}

				case ChartTextOrientation.RegionUp:
					{

						if (m_series.RequireInvertedAxes)
						{
							pt.X -= sz.Width;
						}

						else
						{
							pt.Y += sz.Height;
						}
						break;
					}

				case ChartTextOrientation.RegionDown:
					{
						float f = GetLowerAnchorPointValue(index);

						if (m_series.RequireInvertedAxes)
						{
							pt.X = f;
						}

						else
						{
							pt.Y = f - sz.Height;
						}
						break;
					}

				case ChartTextOrientation.RegionCenter:
					{
						float f = GetLowerAnchorPointValue(index);

						if (m_series.RequireInvertedAxes)
						{
							pt.X = f + (pt.X - f - sz.Width / 2) / 2;
						}

						else
						{
							pt.Y = f - (f - pt.Y + sz.Height / 2) / 2;
						}
						break;
					}

				case ChartTextOrientation.SymbolCenter:
					{
						PointF point = this.GetSymbolPoint(styledPoint);
						pt.X = point.X - sz.Width / 2;
						pt.Y = point.Y - sz.Height / 2;
						break;
					}
			}
			#endregion

			Rectangle car = ChartArea.RenderBounds;
			float angle = (float)Math.PI * style.Font.Orientation / 180.0f;

			if (angle != 0)
			{
				float pT = car.Top - pt.Y;
				if (pT > 0 && Math.Abs(pT) < sz.Width)// here we check whether drawing (and rotation) origin didn't pass the bounds.
				{
					pt.X = car.Top;
					pT = 0;
				}
				float pB = car.Bottom - pt.Y;
				if (pB < 0 && Math.Abs(pB) < sz.Width)// here we check whether drawing (and rotation) origin didn't pass the bounds.
				{
					pt.X = car.Bottom;
					pB = 0;
					pT = -car.Height;
				}
				float pL = car.Left - pt.X;
				if (pL > 0 && Math.Abs(pL) < sz.Width)// here we check whether drawing (and rotation) origin didn't pass the bounds.
				{
					pt.X = car.Left;
					pL = 0;
				}
				float pR = car.Right - pt.X;
				if (pR < 0 && Math.Abs(pR) < sz.Width)// here we check whether drawing (and rotation) origin didn't pass the bounds.
				{
					pt.X = car.Right;
					pR = 0;
					pL = -car.Width;
				}

				float wT = pT / (float)Math.Sin(angle);
				if (wT < 0.0f) wT = float.MaxValue;

				float wB = pB / (float)Math.Sin(angle);
				if (wB < 0.0f) wB = float.MaxValue;

				float wL = pL / (float)Math.Cos(angle);
				if (wL < 0.0f) wL = float.MaxValue;

				float wR = pR / (float)Math.Cos(angle);
				if (wR < 0.0f) wR = float.MaxValue;

				float minWidth = Math.Min(wT, wB);
				minWidth = Math.Min(wL, minWidth);
				minWidth = Math.Min(wR, minWidth);

				sz = g.Graphics.MeasureString(style.Text, style.GdipFont, (int)minWidth - style.GdipFont.Height / 2).ToSize();
				sz.Width += 1;
			}

			pt.X += -sz.Width / 2 + offset.X;
			pt.Y += -sz.Height / 2 + offset.Y;

			if (angle == 0)
			{
				float pT = car.Top - pt.Y;
				float pB = car.Bottom - pt.Y;
				float pL = car.Left - pt.X;
				float pR = car.Right - (pt.X + sz.Width);

				if (pT > 0 && Math.Abs(pT) < sz.Height)
					pt.Y = car.Top;
				if (pB < 0 && Math.Abs(pB) < sz.Height)
					pt.Y = car.Bottom - sz.Height;
				if (pL > 0 && Math.Abs(pL) < sz.Width)
				{
					pt.X = car.Left;
					sz = g.Graphics.MeasureString(style.Text, style.GdipFont, (int)(sz.Width - pL)).ToSize();
				}
				if ((pR < 0) && (Math.Abs(pR) < sz.Width))
				{
					sz = g.Graphics.MeasureString(style.Text, style.GdipFont, (int)(sz.Width + pR)).ToSize();
				}
				sz.Width += 1;
			}

			Brush brush = new SolidBrush(style.TextColor);
			GraphicsPath tgp = new GraphicsPath();
			Font fnt = style.Font.GdipFont;

			if (m_series.SmartLabels)
			{
				pt = m_labelLayoutManager.AddLabel(new ChartLabel(pt, po, sz, SizeF.Empty)).Location;
			}

			if (!pt.IsEmpty)
			{
				if (style.Font.Orientation == 0)
				{
					RectangleF r = new RectangleF(pt, sz);
					tgp.AddString(style.Text, fnt.FontFamily, (int)fnt.Style, RenderingHelper.GetFontSizeInPixels(fnt), r, StringFormat.GenericDefault);
				}
				else
				{
					Matrix mtr = new Matrix();
					mtr.Translate(pt.X + sz.Width / 2, pt.Y + sz.Height / 2f);
					mtr.Rotate(style.Font.Orientation, MatrixOrder.Prepend);
					RectangleF r = new RectangleF(new PointF(style.TextOffset, -sz.Height / 2f), sz);
					tgp.AddString(style.Text, fnt.FontFamily, (int)fnt.Style, RenderingHelper.GetFontSizeInPixels(fnt),
						r, StringFormat.GenericDefault);
					tgp.Transform(mtr);
				}

				g.AddPolygon(Path3D.FromGraphicsPath(tgp, p.Z - 1, brush));
			}
		}
		/// <summary>
		/// Overloaded. Renders elements such as Text and Point Symbols.
		/// </summary>
		/// <param name="g">The graphics object that is to be used.</param>
		protected internal virtual void RenderAdornments(Graphics g)
		{
			if (m_series.BaseType != ChartSeriesBaseType.Single)
			{
				IndexRange visibleRange = this.CalculateVisibleRange();
				ChartStyledPoint[] styledPoints = this.PrepearePoints();

				if (m_series.SmartLabels)
				{
					m_labelLayoutManager = new ChartLabelLayoutManager(m_bounds);
					SizeF minsz = new SizeF(float.MaxValue, float.MaxValue);

					for (int i = visibleRange.From; i <= visibleRange.To; i++)
					{
						if (styledPoints[i].IsVisible)
						{
							ChartStyleInfo style = styledPoints[i].Style;
							SizeF sz = g.MeasureString(style.Text, style.GdipFont).ToSize();

							if (minsz.Width > sz.Width) minsz.Width = sz.Width;
							if (minsz.Height > sz.Height) minsz.Height = sz.Height;
						}
					}

					m_labelLayoutManager.MinimalSize = minsz;

					for (int i = visibleRange.From; i <= visibleRange.To; i++)
					{
						if (styledPoints[i].IsVisible)
						{
							m_labelLayoutManager.AddPoint(this.GetPointFromValue(styledPoints[i]));
						}
					}
				}

				for (int i = visibleRange.From; i <= visibleRange.To; i++)
				{
					if (styledPoints[i].IsVisible)
					{
						this.RenderAdornment(g, styledPoints[i]);
					}
				}
			}
		}
		/// <summary>
		/// Renders elements such as Text and Point Symbols.
		/// </summary>
		/// <param name="g">The graphics object that is to be used.</param>
		protected internal virtual void RenderAdornments(Graphics3D g)
		{
			m_labelLayoutManager = new ChartLabelLayoutManager(m_bounds);

			if (m_series.BaseType != ChartSeriesBaseType.Single)
			{
				IndexRange visibleRange = this.CalculateVisibleRange();
				ChartStyledPoint[] styledPoints = this.PrepearePoints();

				for (int i = visibleRange.From; i <= visibleRange.To; i++)
				{
					ChartStyledPoint styledPoint = styledPoints[i];
					ChartStyleInfo style = styledPoint.Style;

					if (styledPoint.IsVisible)
						{
						if (style.Symbol.Shape != ChartSymbolShape.None)
						{
							this.DrawPointSymbol(g, styledPoint);
						}

						if (style.DisplayText && style.Text != string.Empty)
						{
							Vector3D v3 = GetSymbolVector(styledPoint);
							Size sz = g.Graphics.MeasureString(style.Text, style.GdipFont).ToSize();
							DrawText(g, styledPoint, v3, sz);
						}
					}
				}
			}
		}

		/// <summary>
		/// Renders the adornment.
		/// </summary>
		/// <param name="g">The g.</param>
		/// <param name="point">The point.</param>
		protected virtual void RenderAdornment(Graphics g, ChartStyledPoint point)
		{
			ChartStyleInfo style = point.Style;
			PointF symbolPoint = PointF.Empty;

			if (style.Symbol.Shape != ChartSymbolShape.None)
			{
				symbolPoint = this.GetSymbolPoint(point);
                
                if (this.Chart.HighlightSymbol)
                {
                    BrushInfo brush = this.GetBrush(point.Index, style.Symbol.Color);
                    RenderingHelper.DrawPointSymbol(g, point.Style, symbolPoint, false, brush);
                }
                else
                {
                    RenderingHelper.DrawPointSymbol(g, point.Style, symbolPoint, false); 
                }

				if (m_chart.NeedRegionUpdate)
				{
					Size sblSize = point.Style.Symbol.Size;
					RectangleF rect = new RectangleF(symbolPoint.X - 0.5f * sblSize.Width,
						symbolPoint.Y - 0.5f * sblSize.Height, sblSize.Width, sblSize.Height);
					m_chart.ChartRegions.Add(new ChartRegion(new Region(rect),
						m_chart.Series.IndexOf(m_series), point.Index, point.ToolTip, "Symbol"));
				}
			}

			if (style.DisplayText && style.Text != string.Empty)
			{
				if (symbolPoint.IsEmpty)
				{
					symbolPoint = this.GetSymbolPoint(point);
				}

               this.DrawText(g, point, symbolPoint);
			}
		}
		#endregion

		#region Gets ToolTips
		/// <summary>
		/// Gets the tool tip.
		/// </summary>
		/// <returns></returns>
		protected string GetToolTip()
		{
			return String.Format(m_series.SeriesToolTipFormat, m_series.Name, SeriesStyle.ToolTip);
		}

		/// <summary>
		/// Gets the tooltip by the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		protected string GetToolTip(int index)
		{
			StringBuilder strBuilder = new StringBuilder();
			string format = m_series.PointsToolTipFormat;
			string res = "";

			if (index >= 0)
			{
				object[] args = new object[4 + m_series.Points[index].YValues.Length];

				args[0] = m_series.Name;
				args[1] = SeriesStyle.ToolTip;
				args[2] = GetStyleAt(index).ToolTip;
				args[3] = m_series.Points[index].X;

				for (int i = 4; i < args.Length; i++)
				{
					args[i] = m_series.Points[index].YValues[i - 4];
				}

                res = m_series.Style.ToolTipFormat == string.Empty ? String.Format(m_series.PointsToolTipFormat, args) : String.Format(m_series.Style.ToolTipFormat, args[4]);
			}

			return res == "" ? GetToolTip() : res;
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="chart"></param>
		/// <param name="bounds"></param>
		/// <param name="xAxis"></param>
		/// <param name="yAxis"></param>
		internal void SetBoundsAndRange(IChartAreaHost chart, RectangleF bounds, ChartAxis xAxis, ChartAxis yAxis)
		{
			m_chart = chart;
			m_bounds = bounds;
			m_xAxis = xAxis;
			m_yAxis = yAxis;

			//Debug.WriteLine("SetBoundsAndRange called");
		}

		/// <summary>
		/// Creates the space separator.
		/// </summary>
		/// <param name="z">The Z coordinate.</param>
		/// <returns></returns>
		protected Polygon CreateBoundsPolygon(float z)
		{
			return new Polygon(new Vector3D[]{ new Vector3D( m_bounds.Left, m_bounds.Top, z ),
                                          new Vector3D( m_bounds.Right, m_bounds.Top, z ),
                                          new Vector3D( m_bounds.Right, m_bounds.Bottom, z ),
                                          new Vector3D( m_bounds.Left, m_bounds.Bottom, z ) });
		}

		#region Gets the styles
		/// <summary>
		/// Delegates to <see cref="ChartSeries.GetOfflineStyle()"/> to return the style associated with this index.
		/// You can use this override to specify additional style attributes on a renderer basis.
		/// </summary>
		/// <param name="index">Index value of the point for which the style is required.</param>
		/// <returns>Offline composed copy of the style associated with the index.</returns>
		protected virtual ChartStyleInfo GetStyleAt(int index)
		{
			ChartStyleInfo st = null;

			if (m_series.EnableStyles)
			{
				if (m_shouldUpdate)
				{
					FillStyles();
				}

				if (index < m_styles.Count)
				{
					st = m_styles[index] as ChartStyleInfo;
				}
				else if (index < m_series.Points.Count)
				{
					st = m_series.GetOfflineStyle(index);
				}
				else
				{
					st = this.SeriesStyle;
				}
			}
			else
			{
				st = this.SeriesStyle;
			}

			return st;
		}

        /// <summary>
        /// Fills the styles.
        /// </summary>
		internal void FillStyles()
		{
			m_styleUpdating = true;
			m_styles.Clear();

			for (int i = 0, len = m_series.Points.Count; i < len; i++)
			{
				ChartStyleInfo style = m_series.GetOfflineStyle(i);

				m_styles.Add(style);

				IList<ChartStyledPoint> cachedPoints = this.GetCache();

				if (cachedPoints.Count > i && cachedPoints[i] != null)
				{
					m_pointsCache[i].Style = style;
				}

				m_shouldUpdate = false;
			}

			m_styleUpdating = false;
		}
		#endregion

		/// <summary>
		/// Determines whether the specified point is visible.
		/// </summary>
		/// <param name="cpt">The ChartPoint instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified point is visible; otherwise, <c>false</c>.
		/// </returns>
		protected bool IsVisiblePoint(ChartPoint cpt)
		{
			return !cpt.IsEmpty && (cpt.YValues != null) && (cpt.YValues.Length >= this.RequireYValuesCount);
		}

		/// <summary>
		/// Generates the array of points with specified style.
		/// </summary>
		/// <returns></returns>
		protected ChartStyledPoint[] PrepearePoints()
		{
			if (m_points == null)
			{
				double lastValue = double.MinValue;

				m_isSorted = true;

                m_chartPerformance = this.Chart.ImprovePerformance;

                this.ResetCache();
                
                this.GetCache();

				for (int i = 0; i < m_pointsCache.Count; i++)
				{
					ChartStyledPoint stlPoint = m_pointsCache[i] as ChartStyledPoint;

					if (stlPoint == null)
					{
                        stlPoint= m_chartPerformance ? this.CreateStyledPoint(m_pointIndex[i]) : this.CreateStyledPoint(i);
						m_pointsCache[i] = stlPoint;
					}

					if (m_isSorted)
					{
						m_isSorted = stlPoint.X > lastValue;
						lastValue = stlPoint.X;
					}
				}

				m_points = m_pointsCache.ToArray();

				if (!m_isSorted && this.ShouldSort)
				{
					Array.Sort(m_points, new ChartStyledPointComparer());
					m_isSorted = true;
				}
			}
			else if (m_series.EnableStyles && m_shouldUpdate)
			{
				foreach (ChartStyledPoint stylePoint in m_points)
				{
					stylePoint.Style = this.GetStyleAt(stylePoint.Index);
					stylePoint.ToolTip = this.GetToolTip(stylePoint.Index);
				}
			}

			return m_points;
		}

		/// <summary>
		/// Creates the styled point.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		protected ChartStyledPoint CreateStyledPoint(int index)
		{
			ChartPoint chartPoint = m_series.Points[index];
			ChartStyledPoint result = new ChartStyledPoint(chartPoint, index);

			result.X = this.GetIndexValueFromX(chartPoint.X);
			result.YValues = chartPoint.YValues;		
			result.Style = this.GetStyleAt(index);
            result.IsVisible = this.IsVisiblePoint(chartPoint);
            if (!m_chartPerformance)
            result.ToolTip = this.GetToolTip(index);
            return result;
		}

		/// <summary>
		/// Calculate the visible indices of points for rendering.
		/// </summary>
		/// <returns></returns>
		protected IndexRange CalculateVisibleRange()
		{
            ChartStyledPoint[] points = this.StyledPoints;

			int from = 0;
			int to = points.Length - 1;

			int iteration = 0;

			if (m_isSorted)
			{
				MinMaxInfo vRange = m_series.ActualXAxis.VisibleRange;

				double min = vRange.Min;
				double max = vRange.Max;

				if (m_series.ActualXAxis.ValueType == ChartValueType.Logarithmic)
				{
					min = Math.Pow(m_series.ActualXAxis.LogBase, min);
					max = Math.Pow(m_series.ActualXAxis.LogBase, max);
				}

				int min1 = from;
				int min2 = to;

				#region Calculate FROM
				for (int middle = (min2 + min1) >> 1; ; middle = (min2 + min1) >> 1)
				{
					iteration++;
					if (min2 - min1 <= 1)
					{
						from = min1;
						break;
					}

					if (points[min1].X >= min)
					{
						from = min1;
						break;
					}

					if (points[min2].X == min)
					{
						from = min2;
						break;
					}

					if (points[middle].X == min)
					{
						from = middle;
						break;
					}
					else if (points[middle].X < min)
					{
						min1 = middle + 1;
					}
					else
					{
						min2 = middle - 1;
					}
				}
				#endregion

				int max1 = from;
				int max2 = to;

				#region Calculate TO
				for (int middle = (max2 + max1) >> 1; ; middle = (max2 + max1) >> 1)
				{
					iteration++;
					if (max2 - max1 <= 1)
					{
						to = max2;
						break;
					}

					if (points[max1].X == max)
					{
						to = max1;
						break;
					}

					if (points[max2].X <= max)
					{
						to = max2;
						break;
					}

					if (points[middle].X == max)
					{
						to = middle;
						break;
					}
					else if (points[middle].X < max)
					{
						max1 = middle + 1;
					}
					else
					{
						max2 = middle - 1;
					}
				}
				#endregion
			}

			return new IndexRange(Math.Max(0, from - 1), Math.Min(points.Length - 1, to + 1));
		}

		/// <summary>
		/// Computes the array of <see cref="IndexRange"/>, using for indicating unempty points.
		/// </summary>
		/// <param name="vrange">The visible range of points.</param>
		/// <returns></returns>
		protected IndexRange[] CalculateUnEmptyRanges(IndexRange vrange)
		{
			ArrayList ranges = new ArrayList();
			int lastIndex = 0;

			for (int i = vrange.From, ci = vrange.To + 1; i < ci; i++)
			{
				if (this.IsVisiblePoint(m_series.Points[i]))
				{
					if (lastIndex == -1)
					{
						lastIndex = i;
					}
				}
				else
				{
					ranges.Add( new IndexRange( lastIndex, i ));
					lastIndex = -1;
				}
		  }

			if (ranges.Count == 0)
			{
				ranges.Add(vrange);
			}

			return (IndexRange[])ranges.ToArray(typeof(IndexRange));
		}

		#region shading methods
		/// <summary>
		/// Gets the phong shading blend.
		/// </summary>
		/// <param name="ambientColor">Color of the ambient.</param>
		/// <param name="diffusiveColor">Color of the diffusive.</param>
		/// <param name="lightColor">Color of the light.</param>
		/// <param name="alpha">The alpha.</param>
		/// <param name="phong_alpha">The phong_alpha.</param>
		/// <param name="colors">The colors.</param>
		/// <param name="positions">The positions.</param>
		internal static void PhongShadingColors(Color ambientColor, Color diffusiveColor, Color lightColor, double alpha, double phong_alpha, out Color[] colors, out float[] positions)
		{
			double R = 0.5;

			int colornum = 10;
			Color i_a = ambientColor;
			Color i_d = diffusiveColor;
			Color i_s = lightColor;

			double ka = 0.45f;
			double kd = 0.55f;
			double ks = 0.9f;
			double dpos = 1.0 / (colornum - 1);


			positions = new float[colornum];
			colors = new Color[colornum];
			for (int i = 0; i < colornum; i++)
			{
				double pos = i * dpos;
				double beta = Math.Asin((pos - R) / R);
				double x = R + R * Math.Sin(beta);

				double r = 0, g = 0, b = 0;

				r = ka * i_a.R;
				g = ka * i_a.G;
				b = ka * i_a.B;

				double t = Math.Max(Math.Cos(alpha + beta), 0);
				r += kd * i_d.R * t;
				g += kd * i_d.G * t;
				b += kd * i_d.B * t;

				//t = Math.Pow(Math.Abs(Math.Cos(alpha / 2 + beta)), phong_alpha);
				t = Math.Pow(Math.Abs(Math.Cos(alpha + 2 * beta)), phong_alpha);
				if (Math.Abs(alpha + 2 * beta) > Math.PI / 2) t = 0;
				r += ks * i_s.R * t;
				g += ks * i_s.G * t;
				b += ks * i_s.B * t;

				r = Math.Min(255, r);
				g = Math.Min(255, g);
				b = Math.Min(255, b);

				positions[i] = (float)(pos);
				colors[i] = Color.FromArgb(ambientColor.A, (int)r, (int)g, (int)b);
			}
			positions[colornum - 1] = 1;
		}
		#endregion

		/// <summary>
		/// Gets the total depth.
		/// </summary>
		/// <returns></returns>
		internal virtual float GetTotalDepth()
		{
			return m_chart.GetChartArea().Depth;
		}

		#endregion

		#region Cache implementation
		/// <summary>
		/// Gets the points cache.
		/// </summary>
		/// <returns></returns>
		private IList<ChartStyledPoint> GetCache()
		{
			if (m_pointsCache == null)
			{
				this.ResetCache();
			}

			return m_pointsCache;
		}

		/// <summary>
		/// Inserts the point.
		/// </summary>
		/// <param name="index">The index.</param>
		private void InsertPoint(int index)
		{
			if (m_pointsCache == null)
			{
				this.GetCache();
			}
			else
			{
				m_pointsCache.Insert(index, null);
			}
		}

		/// <summary>
		/// Removes the point.
		/// </summary>
		/// <param name="index">The index.</param>
		private void RemovePoint(int index)
		{
			if (m_pointsCache == null)
			{
				this.GetCache();
			}
			else
			{
				m_pointsCache.RemoveAt(index);

				foreach (ChartStyledPoint point in m_pointsCache)
				{
					if (point != null && point.Index > index)
					{
						point.Index--;
						point.Point = m_series.Points[point.Index];
					}
				}
			}
		}

		/// <summary>
		/// Updates the point.
		/// </summary>
		/// <param name="index">The index.</param>
		private void UpdatePoint(int index)
		{
			if (m_pointsCache != null)
			{
				m_pointsCache[index] = null;
			}
		}

		/// <summary>
		/// Resets the cache.
		/// </summary>
		private void ResetCache()
		{
                 
			if (m_pointsCache == null)
			{
				m_pointsCache = new List<ChartStyledPoint>(m_series.Points.Count);
			}
            m_pointsCache.Clear();

            if (m_chart != null && m_chartPerformance)
            {
                CalCachePoints();
            }
            else
            {
                for (int i = 0; i < m_series.Points.Count; i++)
                {
                    m_pointsCache.Add(null);
                }
            }
		}

		/// <summary>
		/// Gets the styled point.
		/// </summary>
		/// <param name="pointIndex">Index of the point.</param>
		/// <returns></returns>
		private ChartStyledPoint GetStyledPoint(int pointIndex)
		{
			ChartStyledPoint[] points = this.PrepearePoints();

			for (int i = 0; i < points.Length; i++)
			{
				if( points[i].Index == pointIndex )
					return points[i];
			}

			return null;
		}


        /// <summary>
        /// calculates the points to draw when huge data source binded to chart for improving the performance.
        /// </summary>

        public List<int> CalCachePoints()
        {
            if (m_chartPerformance && m_chart != null)
            {
                m_pointsCache.Clear();
                m_pointIndex.Clear();
                PointF firstPoint = PointF.Empty;
                PointF secondPoint = PointF.Empty;
                ChartPoint second = null;
                bool dropPoints = false;
                dropPoints = m_chart.DropSeriesPoints;
                ChartRenderArgs2D renderArgs = new ChartRenderArgs2D(m_chart, m_series);
                int yIndex = renderArgs.Series.PointFormats[ChartYValueUsage.YValue];
                for (int i = 0; i < m_series.Points.Count; i++)
                {
                    second = m_series.Points[i];
                    secondPoint = renderArgs.GetPoint(second.X, second.YValues[yIndex]);
                    if (!((dropPoints == false && firstPoint.IsEmpty)
                          || ((Math.Abs((firstPoint.X) - secondPoint.X)) > m_series.Resolution)
                            || (Math.Abs((secondPoint.Y) - firstPoint.Y)) > m_series.Resolution))
                    {
                        continue;
                    }
                    else
                    {
                        firstPoint = secondPoint;
                        m_pointsCache.Add(null);
                        m_pointIndex.Add(i);

                    }
                }
            }

            return m_pointIndex;
        }


		#endregion
	}
}

// <copyright file="ChartType.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Windows.Media.Media3D;
    using System.Security.Permissions;
    using Syncfusion.Windows.Shared;
    using System.Linq;

    /// <summary>
    /// Represents ChartAxisLabelInfo
    /// </summary>
    internal struct ChartAxisLabelInfo
    {
        #region Members
        /// <summary>
        /// Initializes m_position
        /// </summary>
        private double m_position;

        /// <summary>
        /// Initializes m_valueToPresent
        /// </summary>
        private double m_valueToPresent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public double Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        /// <summary>
        /// Gets or sets the value to pesent.
        /// </summary>
        /// <value>The value to pesent.</value>
        public double ValueToPresent
        {
            get { return m_valueToPresent; }
            set { m_valueToPresent = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisLabelInfo"/> struct.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="value">The value.</param>
        public ChartAxisLabelInfo(double pos, double value)
        {
            m_position = pos;
            m_valueToPresent = value;
        }
        #endregion
    }

    /// <summary>
    /// This is base class for all chart segments.
    /// </summary>
    /// <remarks>
    /// Segment is a building block for any chart series. 
    /// Depending on chart type, segments on series may change. 
    /// For detailed information about segments on series see inherited segments documentation.
    /// Use chart segments templates to customize series look.
    /// </remarks>
    /// <seealso cref="ChartTypes"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class ChartSegment : DependencyObject, IDisposable
    {
        #region Members
        /// <summary>
        /// Identifies CenterOfViewport attached readonly dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey CenterOfViewportPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("CenterOfViewport", typeof(Point), typeof(ChartSegment), new FrameworkPropertyMetadata(new Point()));

        /// <summary>
        /// Represents IsSelected property key.
        /// </summary>
        internal static readonly DependencyPropertyKey IsSelectedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsSelected", typeof(bool), typeof(ChartSegment), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Represents Series property key.
        /// </summary>
        protected static readonly DependencyPropertyKey SeriesPropertyKey =
            DependencyProperty.RegisterReadOnly("Series", typeof(ChartSeries), typeof(ChartSegment), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Label axis info.
        /// </summary>
        private Nullable<ChartAxisLabelInfo> m_axisLabelInfo = null;

        /// <summary>
        /// Represents X-axis range of segment.
        /// </summary>
        internal DoubleRange xRange = DoubleRange.Empty;

        /// <summary>
        /// Represents Y-axis range of segment. 
        /// </summary>
        internal DoubleRange yRange = DoubleRange.Empty;

        /// <summary>
        /// Represents Z-axis range of segment. 
        /// </summary>
        protected DoubleRange zRange = DoubleRange.Empty;

        /// <summary>
        /// Series points that segment represent.
        /// </summary>
        internal ChartIndexedDataPoint[] seriesCorrespondingPoints;

        /// <summary>
        /// Initializes m_geometry
        /// </summary>
        private GeometryModel3D m_geometry;



        /// <summary>
        /// Initializes m_geometry3DGroup
        /// </summary>
        private Model3DGroup m_geometry3DGroup;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the ToolTip dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolTipProperty = ChartSeries.ToolTipProperty.AddOwner(typeof(ChartSegment));

        /// <summary>
        /// Identifies the Interior dependency property.
        /// </summary>
        public static readonly DependencyProperty InteriorProperty = ChartSeries.InteriorProperty.AddOwner(typeof(ChartSegment));

        /// <summary>
        /// Identifies the Stroke dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = ChartSeries.StrokeProperty.AddOwner(typeof(ChartSegment));

        /// <summary>
        /// Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = ChartSeries.StrokeThicknessProperty.AddOwner(typeof(ChartSegment));

        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>
        protected static readonly DependencyPropertyKey DefaultTemplatePropertyKey =
          DependencyProperty.RegisterReadOnly("DefaultTemplate", typeof(DataTemplate), typeof(ChartSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsHighlighted property.
        /// </summary>
        public static readonly DependencyProperty HighlightedProperty =
            DependencyProperty.Register("Highlighted", typeof(bool), typeof(ChartSegment), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the Item dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(object), typeof(ChartSegment), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies Series dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty = SeriesPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies CenterOfViewport dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterOfViewportProperty = CenterOfViewportPropertyKey.DependencyProperty;

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether IsSelected. This is a dependency property.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
        }

        /// <summary>
        /// Gets a value indicating the parent Series. This is a dependency property.
        /// </summary>
        public ChartSeries Series
        {
            get { return (ChartSeries)GetValue(SeriesProperty); }
        }

        /// <summary>
        /// Gets or sets the Item. This is a dependency property.
        /// </summary>
        /// <value>The Item value.</value>
        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>The geometry.</value>
        public GeometryModel3D Geometry3D
        {
            get
            {
                if (m_geometry == null)
                {
                    m_geometry = new GeometryModel3D();
                }

                return m_geometry;
            }

            set
            {
                if (m_geometry != value)
                {
                    m_geometry = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the geometry 3D group.
        /// </summary>
        /// <value>The geometry 3D group.</value>
        public Model3DGroup Geometry3DGroup
        {
            get
            {
                if (m_geometry3DGroup == null)
                {
                    m_geometry3DGroup = new Model3DGroup();
                }

                return m_geometry3DGroup;
            }

            set
            {
                if (m_geometry3DGroup != value)
                {
                    m_geometry3DGroup = value;
                }
            }
        }

        /// <summary>
        /// Gets the X data measure of series.
        /// </summary>
        /// <remarks>
        /// Used to determine X data range that segment represents.
        /// </remarks>
        /// <value>The X data measure.</value>
        public DoubleRange XDataMeasure
        {
            get { return xRange; }
        }

        /// <summary>
        /// Gets the Y data measure of series.
        /// </summary>
        /// <remarks>
        /// Used to determine Y data range that segment represents.
        /// </remarks>
        /// <value>The Y data measure.</value>
        public DoubleRange YDataMeasure
        {
            get { return yRange; }
        }

        /// <summary>
        /// Get the CLR property ZDataMeasure from the internal variable zRange
        /// </summary>
        public DoubleRange ZDataMeasure
        {
            get { return zRange; }
        }

        /// <summary>
        /// Gets the corresponding points.
        /// </summary>
        /// <remarks>
        /// Represents points that segments should be built on.
        /// </remarks>
        /// <value>The <see cref="ChartIndexedDataPoint"/> points array.</value>
        public ChartIndexedDataPoint[] CorrespondingPoints
        {
            get
            {
                return seriesCorrespondingPoints;
            }
        }

        /// <summary>
        /// Gets or sets the tooltip.
        /// </summary>
        /// <value>The tool tip.</value>
        public ToolTip ToolTip
        {
            get { return (ToolTip)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interior brush. This is a dependency property. 
        /// </summary>
        /// <remarks>
        /// This property is being set automatically by the chart building system.
        /// This property in bound with similar property on series that segment belongs to.
        /// </remarks>
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke brush. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property in bound with similar property on series that segment belongs to.
        /// </remarks>
        /// <value>The stroke <see cref="Brush"/>.</value>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stroke thickness. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This property in bound with similar property on series that segment belongs to.
        /// </remarks>
        /// <value>The stroke thickness.</value>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets the default segment template. This is a dependency property.
        /// </summary>
        /// <value>The <see cref="DataTemplate"/>.</value>
        public DataTemplate DefaultTemplate
        {
            get
            {
                if (this.Series.EnableEffects == true && !(this is ChartEmptySymbolSegment))
                {
                    string str = this.Series.Type.ToString();
                    switch (this.Series.Type)
                    {
                        case ChartTypes.Column:
                        case ChartTypes.Bar:
                        case ChartTypes.StackingColumn:
                        case ChartTypes.StackingColumn100:
                        case ChartTypes.StackingBar:
                        case ChartTypes.StackingBar100:
                        case ChartTypes.Gantt:
                        case ChartTypes.BoxAndWhisker:
                        case ChartTypes.Candle:
                        case ChartTypes.RangeColumn:
                        case ChartTypes.Histogram:
                            str = "Column";
                            break;
                        case ChartTypes.Pie:
                        case ChartTypes.Doughnut:
                            str = "Pie";
                            break;
                    }

                    DataTemplate effectsTemplate = (DataTemplate)ChartDataUtils.GetResourceByString(str);
                    if (effectsTemplate == null)
                    {
                        return (DataTemplate)GetValue(DefaultTemplatePropertyKey.DependencyProperty);
                    }
                    else
                    {
                        return effectsTemplate;
                    }
                }
                else
                {
                    return (DataTemplate)GetValue(DefaultTemplatePropertyKey.DependencyProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this segment is highlighted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this segment is highlighted; otherwise, <c>false</c>.
        /// </value>
        public bool Highlighted
        {
            get { return (bool)GetValue(HighlightedProperty); }
            set { SetValue(HighlightedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the axis label info.
        /// </summary>
        /// <value>The axis label info.</value>
        internal Nullable<ChartAxisLabelInfo> AxisLabelInfo
        {
            get
            {
                return m_axisLabelInfo;
            }

            set
            {
                m_axisLabelInfo = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSegment"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        protected ChartSegment(ChartSeries series, ChartIndexedDataPoint[] correspondingPoints)
        {
            SetValue(SeriesPropertyKey, series);
            seriesCorrespondingPoints = correspondingPoints;

            bool m_isColorEach = series.ColorEach.HasValue ? (bool)series.ColorEach : false ;
			if(m_isColorEach && series.ColorEachDependent)
                series.UpdateColorEachSegments(series, this, CorrespondingPoints[0].Index, null);
            else
            {
                BindingUtils.SetBinding(this, series, InteriorProperty, ChartSeries.InteriorProperty);
                BindingUtils.SetBinding(this, series, StrokeProperty, ChartSeries.StrokeProperty);
            }
            BindingUtils.SetBinding(this, series, StrokeThicknessProperty, ChartSeries.StrokeThicknessProperty);
            BindingUtils.SetBinding(this, series, ToolTipProperty, ChartSeries.ToolTipProperty);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSegment"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="adornment"></param>
        protected ChartSegment(ChartSeries series, ChartIndexedDataPoint[] correspondingPoints, ChartAdornmentInfo adornment)
        {
            SetValue(SeriesPropertyKey, series);
            seriesCorrespondingPoints = correspondingPoints;
                        
                BindingUtils.SetBinding(this, series, InteriorProperty, ChartSeries.InteriorProperty);
                BindingUtils.SetBinding(this, series, StrokeProperty, ChartSeries.StrokeProperty);
                BindingUtils.SetBinding(this, series, StrokeThicknessProperty, ChartSeries.StrokeThicknessProperty);
           
              
        }

        /// <summary>
        /// Initializes static members of the <see cref="ChartSegment"/> class.
        /// </summary>
        static ChartSegment()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(ChartSegment));
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Gets the value of the CenterOfViewport dependency property.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>the CenterOfViewport point</returns>
        public static Point GetCenterOfViewport(DependencyObject obj)
        {
            return (Point)obj.GetValue(CenterOfViewportProperty);
        }

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public virtual void Update(IChartTransformer transformer)
        {

            if (this.Interior!= null && this.Interior.CanFreeze && this.Series.ShowEmptyPoints == false)
            {
               this.Interior.Freeze();
            }

            if (this.Stroke != null && this.Stroke.CanFreeze && this.Series.ShowEmptyPoints == false)
            {
               this.Stroke.Freeze();
            }

            Series.SetValue(CenterOfViewportPropertyKey, ChartLayoutUtils.GetCenter(transformer.Viewport));
        }

        /// <summary>
        /// Draw3s the D segment.
        /// </summary>
        /// <param name="transformer">The transformer.</param>
        public virtual void Draw3DSegment(IChartTransformer transformer)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the X range for segment.
        /// </summary>
        /// <param name="values">The values.</param>
        protected void SetXRange(params double[] values)
        {
            if(this.Series.ActualXAxis!=null)
            //if ((this.Series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.Default) || this.Series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.AdjustAcrossChartTypes)
            //    values = SetPaddingforDefaultandAdjustAcrossChartTypes(this.Series.Type, values);
              xRange = DoubleRange.Union(values);
        }

        /// <summary>
        /// Sets the Y range for segment.
        /// </summary>
        /// <param name="values">The values.</param>
        protected void SetYRange(params double[] values)
        {
            yRange = DoubleRange.Union(values);
        }

        /// <summary>
        /// Sets the Z range for segment.
        /// </summary>
        /// <param name="values">The values.</param>
        protected void SetZRange(params double[] values)
        {
            zRange = DoubleRange.Union(values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="series"></param>
        internal void SetRange(ChartSeries series)
        {

            if (series.AutoDiscard == AutoDiscardType.None)
                return;

            switch (series.ActualXAxis.ValueType)
            {

                case ChartValueType.Double:
                    {
                        if (series.ActualXAxis.IsAutoSetRange == true)
                        {
                            return;
                        }


                        if (xRange.End >= series.ActualXAxis.VisibleRange.End)
                        {
                            if (series.AutoDiscard == AutoDiscardType.ResetRange)
                            {
                                double range = series.ActualXAxis.InternalRange.End - series.ActualXAxis.InternalRange.Start;
                                DoubleRange newRange = new DoubleRange(xRange.End, xRange.End + range);
                                SetPointsForAllSeries(series, series.Area.PrimaryAxis);
                                series.ActualXAxis.InternalRange = newRange;
                            }
                            else if (series.AutoDiscard == AutoDiscardType.ExtendRange)
                            {
                                DoubleRange newRange = new DoubleRange(series.ActualXAxis.InternalRange.Start, 2 * series.ActualXAxis.InternalRange.End - series.ActualXAxis.Range.Start);
                                SetPointsForAllSeries(series, series.Area.PrimaryAxis);
                                series.ActualXAxis.InternalRange = newRange;
                            }

                        }

                        if (series.ActualYAxis.IsAutoSetRange == true)
                        {
                            return;
                        }

                    }
                    break;
                case ChartValueType.DateTime:
                    {
                        if (series.ActualXAxis.IsAutoSetRange == true)
                        {
                            return;
                        }


                        if (xRange.End >= series.ActualXAxis.VisibleRange.End)
                        {

                            if (series.AutoDiscard == AutoDiscardType.ResetRange)
                            {
                                TimeSpan range = series.ActualXAxis.DateTimeRange.End - series.ActualXAxis.DateTimeRange.Start;
                                int sec = (int)((range.Seconds + range.Minutes * 60 + range.Hours * 3600) / 60);

                                DateTime startTime = series.ActualXAxis.DateTimeRange.Start + range;

                                DateTime endTime = startTime + range;
                                DateTimeRange newDTrange = new DateTimeRange(startTime, endTime);
                                SetPointsForAllSeries(series, series.Area.PrimaryAxis);
                                series.ActualXAxis.DateTimeRange = newDTrange;

                            }
                            else if (series.AutoDiscard == AutoDiscardType.ExtendRange)
                            {
                                TimeSpan range = series.ActualXAxis.DateTimeRange.End - series.ActualXAxis.DateTimeRange.Start;
                                DateTime endTime = series.ActualXAxis.DateTimeRange.End + range;
                                DateTimeRange newDTrange = new DateTimeRange(series.ActualXAxis.DateTimeRange.Start, endTime);
                                SetPointsForAllSeries(series, series.Area.PrimaryAxis);
                                series.ActualXAxis.DateTimeRange = newDTrange;
                            }

                        }
                        if (series.ActualYAxis.IsAutoSetRange == true)
                        {
                            return;
                        }
                    }
                    break;
                case ChartValueType.String:
                    break;
                default:
                    break;
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="series"></param>
        /// <param name="axis"></param>
        protected virtual void SetPointsForAllSeries(ChartSeries series, ChartAxis axis)
        {

        }



        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"></see> has been updated. The specific dependency property that changed is reported in the event data.
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ChartPropertyMetadata propertyMetadata = e.Property.GetMetadata(this.GetType()) as ChartPropertyMetadata;

            if (propertyMetadata != null)
            {
                switch (propertyMetadata.Options)
                {
                    case ChartPropertyMetadataOptions.AffectsUpdate:
                        Series.Invalidate();
                        break;

                    case ChartPropertyMetadataOptions.AffectsRedraw:
                        if (Series.Area != null)
                        {
                            Series.Area.Redraw();
                        }

                        break;
                }
            }

            base.OnPropertyChanged(e);
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            DisposeSegment();
            m_geometry = null;
            m_geometry3DGroup = null;
            SetValue(SeriesPropertyKey, null);
            seriesCorrespondingPoints = null;
            m_axisLabelInfo = null;

        }
        /// <summary>
        /// Method is used to Clear the all segments in the Chart
        /// </summary>
        protected virtual void DisposeSegment()
        {
            if (this.seriesCorrespondingPoints != null)
            {
                foreach (ChartIndexedDataPoint item in seriesCorrespondingPoints)
                {
                    item.DataPoint.Dispose();
                }
                this.seriesCorrespondingPoints = null;
            }

        }
        #endregion
    }

    /// <summary>
    /// Represents chart series points data.
    /// </summary>
    /// <remarks>
    /// Chart adornments are used to show additional information about displaying series.
    /// Adornments are widely used to simplify chart look and give user more idea of information that is
    /// represented via chart.
    /// </remarks>
    /// <example>
    /// XAML:
    /// <code language="XAML">
    /// <!--Chart with Adornments-->
    ///   &lt;sfchart:Chart&gt;
    ///             &lt;sfchart:ChartArea Background="LightGray" GridBackground="White"&gt;  
    ///                 &lt;sfchart:ChartSeries Type="Column" &gt;
    ///                &lt;sfchart:ChartSeries.AdornmentsInfo&gt;
    ///                         &lt;sfchart:ChartAdornmentInfo 
    /// LabelContentPath="DataPoint.X" Visible="True"  /&gt;
    ///                     &lt;/sfchart:ChartSeries.AdornmentsInfo&gt;
    ///                 &lt;/sfchart:ChartSeries&gt;       
    ///             &lt;/sfchart:ChartArea&gt;          
    ///         &lt;/sfchart:Chart&gt;
    /// </code>
    /// C#:
    /// <code language="C#">
    /// ChartSeries series = Chart1.Areas[0].Series[0];      
    /// ChartAdornmentInfo adornments = series.AdornmentsInfo;
    /// adornments.LabelContentPath = "DataPoint.X";
    /// adornments.Visible = true;
    /// </code>
    /// </example>
    /// <seealso cref="AnnotationsCollection"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAdornment : ChartSegment, IDisposable
    {
        #region Members
        /// <summary>
        /// Initializes m_point
        /// </summary>
        internal IChartDataPoint m_point;
        internal object m_labelValue = null;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the X value dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(ChartAdornment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y value dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(ChartAdornment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the DataPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty DataPointProperty =
            DependencyProperty.Register("DataPoint", typeof(IChartDataPoint), typeof(ChartAdornment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Index dependency property.
        /// </summary>
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(ChartAdornment), new UIPropertyMetadata(-1));

        /// <summary>
        /// Identifies the SegmentLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentLabelProperty =
            DependencyProperty.Register("SegmentLabel", typeof(string), typeof(ChartAdornment), new UIPropertyMetadata(string.Empty));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the segment label. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Allows specify value that should be displayed for the segment's adorner.
        /// </remarks>
        /// <example>
        /// This property is not intended to be used from user code. All segment labels are 
        /// assigned automatically. 
        /// </example>
        /// <value>The segment label type of <see cref="String"/>.</value>
        public string SegmentLabel
        {
            get
            {
                return (string)GetValue(SegmentLabelProperty);
            }

            set
            {
                SetValue(SegmentLabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X. This is a dependency property.
        /// </summary>
        /// <example>
        /// This property is not intended to be used from user code. All segment positions are 
        /// assigned automatically with respect to segments. 
        /// </example>
        /// <value>The X value.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y. This is a dependency property.
        /// </summary>
        /// <example>
        /// This property is not intended to be used from user code. All segment positions are 
        /// assigned automatically with respect to segments. 
        /// </example>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data point.
        /// </summary>
        /// <value>The data point.</value>
        internal IChartDataPoint DataPoint
        {
            get { return (IChartDataPoint)GetValue(DataPointProperty); }
            set { SetValue(DataPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAdornment"/> class.
        /// </summary>
        /// <remarks>
        /// Chart adornments are used to show additional information about displaying series.
        /// </remarks>
        /// <param name="position">Adornment's position.</param>
        /// <param name="point">Point to represent.</param>
        /// <param name="series">Connected to series.</param>
        internal ChartAdornment(IChartDataPoint position, ChartIndexedDataPoint point, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { point }, series.AdornmentsInfo)
        {
            m_point = position;

            this.DataPoint = point.DataPoint;
            this.Index = point.Index;
            
            this.SetXRange(m_point.X);
            this.SetYRange(m_point.Values[0]);
        }
        internal ChartAdornment(IChartDataPoint position, ChartIndexedDataPoint point, ChartSeries series, DoubleRange yRange)
            : base(series, new ChartIndexedDataPoint[] { point }, series.AdornmentsInfo)
        {
            m_point = position;
            this.DataPoint = point.DataPoint;
            this.Index = point.Index;
            this.SetXRange(m_point.X);
            this.yRange = yRange;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of the class that implements <see cref="IChartTransformer"/></param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            Point pt = transformer.TransformToVisible(m_point.X, m_point.Values[0]);

            this.X = pt.X;
            this.Y = pt.Y;
        }
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.DataPoint = null;
            this.m_point.Dispose();
            this.m_point = null;
            
        }

        #endregion
    }

    /// <summary>
    /// This is base class for all chart types. 
    /// </summary>
    /// <seealso cref="ChartType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class ChartType : IDisposable
    {
        #region Internal types

        /// <summary>
        /// Chart type flags are used to identify specific to chart settings.
        /// </summary>
        [Flags]
        protected enum ChartTypeFlags
        {
            /// <summary>
            /// Default chart type flag.
            /// </summary>
            None = 0x00,

            /// <summary>
            /// Identifies that chart segments can be represented side-by-side on area.
            /// </summary>
            SideBySide = 0x01,

            /// <summary>
            /// Identifies that chart are represented in stacking form.
            /// </summary>
            Stacked = 0x02,

            /// <summary>
            /// Identifies that chart type is indexed.
            /// </summary>
            Indexed = 0x04,

            /// <summary>
            /// Identifies that chart type is rotated.
            /// </summary>
            Rotated = 0x08,

            /// <summary>
            /// Identifies that axis is not required for this chart type.
            /// </summary>
            NotRequiresAxis = 0x10,

            /// <summary>
            /// Identifies that chart has custom labels on axis.
            /// </summary>
            CustomAxisLabels = 0x20
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for Spacing.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.RegisterAttached("Spacing", typeof(double), typeof(ChartType), new PropertyMetadata(0.2d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this type is indexed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this type is indexed; otherwise, <c>false</c>.
        /// </value>
        public bool IsIndexed
        {
            get
            {
                return CheckFlags(ChartTypeFlags.Indexed);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type is independent
        /// </summary>
        public bool IsRotated
        {
            get
            {
                return CheckFlags(ChartTypeFlags.Rotated);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type is stacked type.
        /// </summary>
        public bool IsStacked
        {
            get
            {
                return CheckFlags(ChartTypeFlags.Stacked);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is side-by-side chart type.
        /// </summary>
        public bool IsSideBySide
        {
            get
            {
                return CheckFlags(ChartTypeFlags.SideBySide);
            }
        }

        /// <summary>
        /// Gets a value indicating whether chart type requires axis to be built.
        /// </summary>
        public bool RequiresAxis
        {
            get
            {
                return !CheckFlags(ChartTypeFlags.NotRequiresAxis);
            }
        }

        /// <summary>
        /// Gets a value indicating whether custom axis labels present.
        /// </summary>
        internal bool CustomAxisLabels
        {
            get
            {
                return this.CheckFlags(ChartTypeFlags.CustomAxisLabels);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type is independent of other series.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is independent; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsIndependent
        {
            get
            {
                return !(CheckFlags(ChartTypeFlags.SideBySide) || CheckFlags(ChartTypeFlags.Stacked));
            }
        }

        /// <summary>
        /// Gets the requirement for data count.
        /// </summary>
        /// <value>The require data count.</value>
        public virtual int RequiresDataCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the required type of the axes.
        /// </summary>
        /// <value>The type of the axes.</value>
        public virtual ChartAxesType AxesType
        {
            get
            {
                return ChartAxesType.CartesianAxes;
            }
        }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        protected abstract ChartTypeFlags Flags { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartType"/> class.
        /// </summary>
        static ChartType()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(ChartType));
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Updates the series in area.
        /// </summary>
        /// <param name="area">The area value.</param>
        internal static void UpdateSeriesInArea(ChartArea area)
        {
            if (area != null)
            {
                foreach (ChartSeries series in area.VisibleSeries)
                {
					if(area.PrimarySeries.Type == ChartTypes.StackingBar100 || area.PrimarySeries.Type == ChartTypes.StackingColumn100)
                        series.Area.SecondaryAxis.CoerceValue(ChartAxis.InternalRangeProperty);
                    series.Invalidate();
                }

                area.UpdateArea();
            }
        }

        /// <summary>
        /// Gets the spacing.
        /// </summary>
        /// <param name="area">The area value.</param>
        /// <returns>The double space</returns>
        public static double GetSpacing(ChartArea area)
        {
            return (double)area.GetValue(SpacingProperty);
        }

        /// <summary>
        /// Sets the spacing.
        /// </summary>
        /// <param name="area">The area value.</param>
        /// <param name="value">The value.</param>
        public static void SetSpacing(ChartArea area, double value)
        {
            area.SetValue(SpacingProperty, value);
        }
        List<ChartIndexedDataPoint> points;
                
        internal static double GetColumnInitialSegmentWidth(ChartSeries series)
        {
            return (double)series.GetValue(ColumnInitialSegmentWidthValueProperty);
        }
 
        internal static void SetColumnInitialSegmentWidthValue(ChartSeries series, double value)
        {
            series.SetValue(ColumnInitialSegmentWidthValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for InitialSegmentStartValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ColumnInitialSegmentWidthValueProperty =
            DependencyProperty.RegisterAttached("ColumnInitialSegmentWidth", typeof(double), typeof(ChartType), new UIPropertyMetadata(0d));

        internal static double GetEndSegmentWidth(ChartSeries series)
        {
            return (double)series.GetValue(EndSegmentWidthProperty);
        }

        internal static void SetEndSegmentWidth(ChartSeries series, double value)
        {
            series.SetValue(EndSegmentWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for EndSegmentWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty EndSegmentWidthProperty =
            DependencyProperty.RegisterAttached("EndSegmentWidth", typeof(double), typeof(ChartType), new UIPropertyMetadata(0d));

        
        /// <summary>
        /// Calculates the segments of specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public virtual void Calculate(ChartSeries series)
        {
            if (points == null || points.Count() ==0 || series.internaldata_modified || series.Type != ChartTypes.FastLine)
            {
                points = GetSeriesVisiblePoints(series);
                series.internaldata_modified = false;
            }
            else
            {
                int length = points.Count-1;
 while(points.Count != series.PointsCount && points.Count < series.PointsCount)
 {
     points.Add(new ChartIndexedDataPoint(series.GetPoint(length), length));
 }
            }
            ChartIndexedDataPoint[] newpoints = points.ToArray();
            if (series.IsSortData == true)
            {
                if (series.IsIndexed == false)
                {
                    switch (series.SortBy)
                    {
                        case SortingAxis.XY:
                        case SortingAxis.X:
                            for (int i = 0; i < newpoints.Length - 1; i++)
                            {
                                for (int j = i; j < newpoints.Length; j++)
                                {
                                    if (series.SortDirection == Direction.Ascending)
                                    {
                                        if (newpoints[i].DataPoint.X > newpoints[j].DataPoint.X)
                                        {
                                            var temp = newpoints[i];
                                            newpoints[i] = newpoints[j];
                                            newpoints[j] = temp;
                                        }
                                    }
                                    else
                                    {
                                        if (newpoints[i].DataPoint.X < newpoints[j].DataPoint.X)
                                        {
                                            var temp = newpoints[i];
                                            newpoints[i] = newpoints[j];
                                            newpoints[j] = temp;
                                        }
                                    }
                                }
                            }
                            break;
                        case SortingAxis.Y:
                            for (int i = 0; i < newpoints.Length - 1; i++)
                            {
                                for (int j = i; j < newpoints.Length; j++)
                                {
                                    if (series.SortDirection == Direction.Ascending)
                                    {
                                        if (newpoints[i].DataPoint.Y > newpoints[j].DataPoint.Y)
                                        {
                                            var temp = newpoints[i];
                                            newpoints[i] = newpoints[j];
                                            newpoints[j] = temp;
                                        }
                                    }
                                    else
                                    {
                                        if (newpoints[i].DataPoint.Y < newpoints[j].DataPoint.Y)
                                        {
                                            var temp = newpoints[i];
                                            newpoints[i] = newpoints[j];
                                            newpoints[j] = temp;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    for (int i = 0; i < newpoints.Length - 1; i++)
                    {
                        for (int j = i; j < newpoints.Length; j++)
                        {
                            if (series.SortDirection == Direction.Ascending)
                            {
                                if (newpoints[i].DataPoint.Y > newpoints[j].DataPoint.Y)
                                {
                                    var temp = newpoints[i];
                                    newpoints[i] = newpoints[j];
                                    newpoints[j] = temp;
                                }
                            }
                            else
                            {
                                if (newpoints[i].DataPoint.Y < newpoints[j].DataPoint.Y)
                                {
                                    var temp = newpoints[i];
                                    newpoints[i] = newpoints[j];
                                    newpoints[j] = temp;
                                }
                            }
                        }
                    }
                }
                ChartIndexedDataPoint[] pt = new ChartIndexedDataPoint[newpoints.Length];
                for (int k = 0; k < newpoints.Length; k++)
                {
                   // points[k].DataPoint.X = k;
                    pt[k] = new ChartIndexedDataPoint(newpoints[k].DataPoint, k);
                }
                newpoints = pt;
            }
            //List<ChartIndexedDataPoint> items = (from pt in newpoints where pt.DataPoint.Y.ToString() != "NaN" select pt).ToList<ChartIndexedDataPoint>();
            //if (items.Count > 0)
            //{
                this.CalculateSegments(series, newpoints);
           // }
            //this.CalculateSegments(series, points);
            //for (int i = 0; i < points.Length; i++)
            //{
            //    if (points[i].DataPoint != null)
            //    {
            //        points[i].DataPoint.Dispose();
            //        points[i].DataPoint.Label = null;
            //        points[i].DataPoint.ParentSegment = null;
            //    }

            //}
            newpoints = null;
        }

        /// <summary>
        /// Updates the specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <seealso cref="ChartType"/>
        public virtual void Update(ChartSeries series)
        {
            if (series != null)
            {
                if (points == null || points.Count() == 0 || series.internaldata_modified || series.Type != ChartTypes.FastLine)
                {
                    points = GetSeriesVisiblePoints(series);
                    series.internaldata_modified = false;
                }
                else
                {
                    int length = points.Count ;
                    while (points.Count != series.PointsCount && points.Count < series.PointsCount )
                    {
                        points.Add(new ChartIndexedDataPoint(series.GetPoint(length), length));
                        length++;
                    }
                }
                ChartIndexedDataPoint[] newpoints = points.ToArray();
                //ChartIndexedDataPoint[] points = GetSeriesVisiblePoints(series);
                if (series.IsSortData == true)
                {
                    if (series.IsIndexed == false)
                    {
                        switch (series.SortBy)
                        {
                            case SortingAxis.XY:
                            case SortingAxis.X:
                                for (int i = 0; i < newpoints.Length - 1; i++)
                                {
                                    for (int j = i; j < newpoints.Length; j++)
                                    {
                                        if (series.SortDirection == Direction.Ascending)
                                        {
                                            if (newpoints[i].DataPoint.X > newpoints[j].DataPoint.X)
                                            {
                                                var temp = newpoints[i];
                                                newpoints[i] = newpoints[j];
                                                newpoints[j] = temp;
                                            }
                                        }
                                        else
                                        {
                                            if (newpoints[i].DataPoint.X < newpoints[j].DataPoint.X)
                                            {
                                                var temp = newpoints[i];
                                                newpoints[i] = newpoints[j];
                                                newpoints[j] = temp;
                                            }
                                        }
                                    }
                                }
                                break;
                            case SortingAxis.Y:
                                for (int i = 0; i < newpoints.Length - 1; i++)
                                {
                                    for (int j = i; j < newpoints.Length; j++)
                                    {
                                        if (series.SortDirection == Direction.Ascending)
                                        {
                                            if (newpoints[i].DataPoint.Y > newpoints[j].DataPoint.Y)
                                            {
                                                var temp = newpoints[i];
                                                newpoints[i] = newpoints[j];
                                                newpoints[j] = temp;
                                            }
                                        }
                                        else
                                        {
                                            if (newpoints[i].DataPoint.Y < newpoints[j].DataPoint.Y)
                                            {
                                                var temp = newpoints[i];
                                                newpoints[i] = newpoints[j];
                                                newpoints[j] = temp;
                                            }
                                        }
                                    }
                                }
                                break;

                        }

                    }
                    else
                    {
                        for (int i = 0; i < newpoints.Length - 1; i++)
                        {
                            for (int j = i; j < newpoints.Length; j++)
                            {
                                if (series.SortDirection == Direction.Ascending)
                                {
                                    if (newpoints[i].DataPoint.Y > newpoints[j].DataPoint.Y)
                                    {
                                        var temp = newpoints[i];
                                        newpoints[i] = newpoints[j];
                                        newpoints[j] = temp;
                                    }
                                }
                                else
                                {
                                    if (newpoints[i].DataPoint.Y < newpoints[j].DataPoint.Y)
                                    {
                                        var temp = newpoints[i];
                                        newpoints[i] = newpoints[j];
                                        newpoints[j] = temp;
                                    }
                                }
                            }
                        }
                        ChartIndexedDataPoint[] pt = new ChartIndexedDataPoint[newpoints.Length];
                        for (int k = 0; k < newpoints.Length; k++)
                        {
                            pt[k] = new ChartIndexedDataPoint(newpoints[k].DataPoint, k);
                            pt[k].DataPoint.X = k;
                        }
                        newpoints = pt;
                    }
                }

                this.UpdateSegments(series, newpoints);
                //this.UpdateLastSegments(series, points);
                //ChartTypes typ = series.Type;
                //int count = newpoints.Length;
                //for (int i = 0; i < count; i++)
                //{
                //    if (newpoints[i].DataPoint != null)
                //    {
                //        newpoints[i].DataPoint.Dispose();
                //        if (typ == ChartTypes.HiLo)
                //        {
                //            newpoints[i].DataPoint.Label = null;
                //        }
                //        //points[i].DataPoint.ParentSegment = null;
                //    }
                //}
                newpoints = null;
            }
        }


        /// <summary>
        /// Determines whether the this chart type is compatible with specified type.
        /// </summary>
        /// <param name="type">The type value.</param>
        /// <returns>
        /// <c>true</c> if the type is compatible; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsCompatible(ChartType type)
        {
            return (this.AxesType == type.AxesType); // && (this.IsRotated == type.IsRotated);
        }
        #endregion

        #region Implementation

        internal List<ChartIndexedDataPoint> indexedPointsList = null;

        /// <summary>
        /// Gets the series visible points.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns><see cref="ChartIndexedDataPoint"/> array.</returns>
        internal List<ChartIndexedDataPoint> GetSeriesVisiblePoints(ChartSeries series)
        {
            List<ChartIndexedDataPoint> indexedPointsList = new List<ChartIndexedDataPoint>();
            bool m_IsLogarithmicXAxis = false;
            bool m_IsLogarithmicYAxis = false;
            m_IsLogarithmicXAxis = series != null && series.XAxis != null ? series.XAxis.IsLogarithmic : false;
            m_IsLogarithmicYAxis = series != null && series.YAxis != null ? series.YAxis.IsLogarithmic : false;

            for (int i = 0, ci = series.PointsCount; i < ci; i++)
            {
                ChartIndexedDataPoint point = new ChartIndexedDataPoint(series.GetPoint(i), i);
                if (point.DataPoint != null)
                {
                    if (point.DataPoint.Item == null)
                    {
                        point.DataPoint.Item = ((Syncfusion.Windows.Chart.ChartPoint)(point.DataPoint)).Tag;
                    }
                    //if (double.IsNaN(point.DataPoint.X) && point.DataPoint.EmptyPoint == true)
                    //{
                    //    point.DataPoint.X = i;
                    //}

                    ////Determining whether point representation should be visible on ChartArea.
                    if (series.IsPointVisible(point.DataPoint, m_IsLogarithmicXAxis, m_IsLogarithmicYAxis) || series.ActualXAxis.IsAutoSetRange == false)
                    {
                        indexedPointsList.Add(point);
                    }
                }
            }
            ////If sorting is required by user, perform sorting
            if (series.IsSortData == true)
            {
                indexedPointsList.Sort(new ChartIndexedDataPointByXComparer());
            }

            return indexedPointsList;//.ToArray();
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected virtual void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartType"/>
        protected virtual void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
        }
        /// <summary>
        /// Calculate the last segment
        /// </summary>
        /// <param name="series"></param>
        /// <param name="points"></param>
        protected virtual void UpdateLastSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
        }
        /// <summary>
        /// Calculates the adornments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="point">The point.</param>
        /// <param name="index">The index.</param>
        /// <returns>The ChartAdornment</returns>
        protected virtual ChartAdornment CreateAdornment(ChartSeries series, ChartIndexedDataPoint point, int index)
        {
            ////  List<ChartSegment> adornments = new List<ChartSegment>();    
            DoubleRange sbsInfo = this.IsSideBySide ?
              series.Area.GetSideBySideInfo(series) : new DoubleRange(0, 0);

            double x = point.DataPoint.X + sbsInfo.Median;
            double y = point.DataPoint.Values[0];

            if (this.IsStacked)
            {
                bool? forPositive = null;
                if ((this is ChartStackingAreaType && ChartStackingAreaType.GetRequiresNegativeSeriesStack(series.Area)) ||
                    (this is ChartStackingLineType && ChartStackingLineType.GetRequiresNegativeSeriesStack(series.Area)) ||
                    (this is ChartStackingSplineType && ChartStackingSplineType.GetRequiresNegativeSeriesStack(series.Area))||
                    (this is ChartStackingSplineAreaType && ChartStackingSplineAreaType.GetRequiresNegativeSeriesStack(series.Area)))
                {
                    forPositive = y >= 0;
                }
                if ((this is ChartStackingColumnType) || this is ChartStackingAreaType || this is ChartStackingLineType || this is ChartStackingSplineType || this is ChartStackingSplineAreaType || this is ChartFastStackingColumnType)
                {
                    if (forPositive != null)
                        y += series.Area.GetStackInfo(series, index, forPositive);
                    else
                    {
                        y = Math.Abs(y);
                        y += Math.Abs(series.Area.GetStackInfo(series, index, forPositive));
                    }
                }
            }

            return new ChartAdornment(new ChartPoint(x, y), point, series);
        }

        internal ChartAdornment CreateAdornment(ChartSeries series, double yVal, object label, double start, double end, ChartIndexedDataPoint point, int index)
        {
            ////  List<ChartSegment> adornments = new List<ChartSegment>();    
            DoubleRange sbsInfo = this.IsSideBySide ?
              series.Area.GetSideBySideInfo(series) : new DoubleRange(0, 0);

            double x = point.DataPoint.X + sbsInfo.Median;
            double y = yVal;
            DoubleRange yRange = new DoubleRange(start, end);
            return new ChartAdornment(new ChartPoint(x, y), point, series, yRange) { m_labelValue = label};
        }

        internal ChartAdornment CreateAdornment(ChartSeries series, double xVal, double yVal, object label, double start, double end, ChartIndexedDataPoint point, int index)
        {
            double x = xVal;
            double y = yVal;
            DoubleRange yRange = new DoubleRange(start, end);
            return new ChartAdornment(new ChartPoint(x, y), point, series, yRange) { m_labelValue = label };
        }
        /// <summary>
        /// Checks the flags.
        /// </summary>
        /// <param name="flag">The flag value.</param>
        /// <returns>The bool value to check flags</returns>
        protected bool CheckFlags(ChartTypeFlags flag)
        {
            return (this.Flags & flag) == flag;
        }
        #endregion


        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (indexedPointsList != null)
            {
                foreach (ChartIndexedDataPoint item in indexedPointsList)
                {
                    if (item.DataPoint != null)
                    {
                        item.DataPoint.Dispose();
                    }
                }
                this.indexedPointsList.Clear();
            }
            if (this.points != null)
            {
                this.points.Clear();
            }
            //if (ChartDataUtils.rd != null)
            //{
            //    ChartDataUtils.rd.MergedDictionaries.Clear();
            //    ChartDataUtils.rd.Clear();
            //    ChartDataUtils.rd = null;
            //}
            
        }

        #endregion
    }

    /// <summary>
    /// Represents the ChartTypeTemplateSelector class
    /// </summary>
    /// <remarks>
    /// Selects the Template required for the chart types
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartTypeTemplateSelector : DataTemplateSelector
    {
        #region Members
        /// <summary>
        /// Initializes m_default
        /// </summary>
        private static ChartTypeTemplateSelector m_default = new ChartTypeTemplateSelector();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        public static ChartTypeTemplateSelector Default
        {
            get { return m_default; }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Overrides <see cref="DataTemplateSelector.SelectTemplate"/> method.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"></see> or null. The default value is null.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ChartSegment segment = item as ChartSegment;

            if (segment != null)
            {
                return segment.DefaultTemplate;
            }

            return null;
        }
        #endregion
    }
}

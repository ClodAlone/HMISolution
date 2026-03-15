#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Globalization;
    using System;
    using Syncfusion.Olap.Engine;

    /// <summary>
    /// Representing OlapChartAxis
    /// </summary>
    public class OlapChartAxis : ChartAxis
    {
        #region Dependency properties
        ///<summary>
        /// Identifies the GroupLineStroke dependency property.
        ///</summary>
        public static readonly DependencyProperty GroupLineStrokeProperty =
            DependencyProperty.Register("GroupLineStroke", typeof(Pen), typeof(OlapChartAxis), new UIPropertyMetadata(new Pen(Brushes.Black, 1)));

        ///<summary>
        /// Identifies the ShowGroupLineStroke dependency property.
        ///</summary>
        public static readonly DependencyProperty ShowGroupLineStrokeProperty =
            DependencyProperty.Register("ShowGroupLineStroke", typeof(bool), typeof(OlapChartAxis), new UIPropertyMetadata(true));
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapChartAxis"/> class.
        /// </summary>
        public OlapChartAxis()
        {
            this.MaximalZoomFactor = 1;
        }

        /// <summary>
        /// Initializes the <see cref="OlapChartAxis"/> class.
        /// </summary>
        static OlapChartAxis()
        {
            ZoomFactorProperty.OverrideMetadata(typeof(OlapChartAxis), new ChartPropertyMetadata(1d, null, CoerceZoomFactor, ChartPropertyMetadataOptions.AffectsUpdate));
        }
        #endregion

        #region Event
        public event OlapMouseEventHandler LabelClick;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the parent area for axis.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// This property is being set by Chart's system internally.
        /// </remarks>
        public new OlapArea Area
        {
            get
            {
                return LogicalTreeHelper.GetParent(this) as OlapArea;
            }
        }

        /// <summary>
        /// Gets or sets the content path.
        /// </summary>
        /// <value>The content path.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string ContentPath
        {
            get { return (string)GetValue(ContentPathProperty); }
            set { SetValue(ContentPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom labels.
        /// </summary>
        /// <value>The custom labels.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ChartAxisLabelsCollection CustomLabels
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the desired intervals count.
        /// </summary>
        /// <value>The desired intervals count.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new int DesiredIntervalsCount
        {
            get;
            set;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool EnableZooming
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the GroupLineStroke. This is a dependency property.
        /// </summary>
        /// <value>The GroupLineStroke.</value>
        public Pen GroupLineStroke
        {
            get { return (Pen)GetValue(GroupLineStrokeProperty); }
            set { SetValue(GroupLineStrokeProperty, value); }
        }

        private ChartLabelIntersectAction _defaultIntersectAction = ChartLabelIntersectAction.Hide;
        /// <summary>
        /// Gets or sets the intersect action.
        /// </summary>
        /// <value>The intersect action.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ChartLabelIntersectAction IntersectAction
        {
            get
            {
                return _defaultIntersectAction;
            }
            set
            {
                _defaultIntersectAction = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new double Interval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the interval offset.
        /// </summary>
        /// <value>The interval offset.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new double IntervalOffset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is auto set range.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is auto set range; otherwise, <c>false</c>.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool IsAutoSetRange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fraction enabled on zoom.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fraction enabled on zoom; otherwise, <c>false</c>.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool IsFractionEnabledOnZoom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is inversed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is inversed; otherwise, <c>false</c>.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool IsInversed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is logarithmic.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is logarithmic; otherwise, <c>false</c>.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool IsLogarithmic
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the label border brush.
        /// </summary>
        /// <value>The label border brush.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Brush LabelBorderBrush
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the label border thickness.
        /// </summary>
        /// <value>The label border thickness.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Thickness LabelBorderThickness
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the label corner radius.
        /// </summary>
        /// <value>The label corner radius.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new CornerRadius LabelCornerRadius
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the label date time format.
        /// </summary>
        /// <value>The label date time format.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string LabelDateTimeFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the label font weight.
        /// </summary>
        /// <value>The label font weight.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new FontWeight LabelFontWeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the label format.
        /// </summary>
        /// <value>The label format.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string LabelFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the labels mode.
        /// </summary>
        /// <value>The labels mode.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ChartAxisLabelsMode LabelsMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the label template.
        /// </summary>
        /// <value>The label template.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new DataTemplate LabelTemplate
        {
            get;
            set;
        }

        private Pen _defaultLineStroke;
        /// <summary>
        /// Gets or sets the line stroke.
        /// </summary>
        /// <value>The line stroke.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Pen LineStroke
        {
            get
            {
                _defaultLineStroke = _defaultLineStroke ?? new Pen(new SolidColorBrush(Color.FromArgb(0xFF, 0X94, 0X94, 0X94)), 1);
                return _defaultLineStroke;
            }
            set
            {
                _defaultLineStroke = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimal zoom factor. Participates in coercing public ZoomFactor property.
        /// </summary>
        /// <value>The minimal zoom factor.</value>
        internal double MaximalZoomFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [opposed position].
        /// </summary>
        /// <value><c>true</c> if [opposed position]; otherwise, <c>false</c>.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool OpposedPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Orientation Orientation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new double Origin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether axis is processing labels state.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if axis is processing labels state; otherwise, <c>false</c>.
        /// </value>
        internal bool ProcessingLabelsState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new DoubleRange Range
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the range padding.
        /// </summary>
        /// <value>The range padding.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ChartRangePaddingType RangePadding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ShowGroupLineStroke. This is a dependency property.
        /// </summary>
        /// <value>The ShowGroupLineStroke.</value>
        public bool ShowGroupLineStroke
        {
            get { return (bool)GetValue(ShowGroupLineStrokeProperty); }
            set { SetValue(ShowGroupLineStrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the small tick.
        /// </summary>
        /// <value>The size of the small tick.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new double SmallTickSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the small ticks per interval.
        /// </summary>
        /// <value>The small ticks per interval.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new int SmallTicksPerInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tick line stroke.
        /// </summary>
        /// <value>The tick line stroke.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Pen TickLineStroke
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the tick.
        /// </summary>
        /// <value>The size of the tick.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new double TickSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ChartValueType ValueType
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Coerces the zoom factor.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoerceZoomFactor(DependencyObject d, object baseValue)
        {
            OlapChartAxis axis = d as OlapChartAxis;
            if (axis != null)
            {
                double requestedZoomFactor = (double)baseValue;
                if (axis.MaximalZoomFactor < requestedZoomFactor)
                {
                    return axis.MaximalZoomFactor;
                }
                return ChartMath.MinMax((double)baseValue, axis.MinimalZoomFactor, 1);
            }
            return baseValue;
        }

        /// <summary>
        /// Raises the label click event.
        /// </summary>
        /// <param name="args">The args.</param>
        internal void RaiseLabelClick(OlapLabelClickEvenArgs args)
        {
            ProcessingLabelsState = true;
            if (LabelClick != null)
            {
                LabelClick(this, args);
            }
            ProcessingLabelsState = false;
        }

        #endregion
    }
    public class PrimaryAxisLabelConvertor : IMultiValueConverter
    {
        /// <summary>
        /// Initializes c_roundDecimals
        /// </summary>

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            this.CellDescriptor = values[0] as OlapLabelPresenter;
            this.OlapChart = values[1] as OlapChart;

            if (this.CellDescriptor != null && this.OlapChart != null)
            {
                string formatString = this.OlapChart.PrimaryAxis.LabelFormat;

                if (!string.IsNullOrEmpty(formatString))
                {
                    PivotCellDescriptor cellDescriptor = this.CellDescriptor.Content as PivotCellDescriptor;
                    if (cellDescriptor != null)
                    {
                        DateTime dateTime = DateTime.Now;
                        if (this.IsFormattedDateTime(formatString) && DateTime.TryParse(cellDescriptor.CellValue, out dateTime))
                        {
                            return string.Format("{0:" + formatString + "}", dateTime);
                        }
                        else
                        {
                            return string.Format("{0:" + formatString + "}", cellDescriptor.CellValue);
                        }
                    }
                }
                else
                {
                    PivotCellDescriptor cellDescriptor = this.CellDescriptor.Content as PivotCellDescriptor;
                    if (cellDescriptor != null)
                    {
                        return cellDescriptor.CellValue;
                    }
                }
            }
            return null;
        }
        private bool IsFormattedDateTime(string formattedString)
        {
            string[] formattedSpecifiers = new string[] { "t", "d", "T", "D", "f", "F", "g", "G", "m", "M", "y", "Y", "r", "R", "s", "u" };
            DateTimeFormatInfo dateTimeInfo = new DateTimeFormatInfo();
            foreach (string formattedType in formattedSpecifiers)
            {
                if (formattedType == formattedString)
                    return true;
            }
            foreach (string datetimePattern in dateTimeInfo.GetAllDateTimePatterns())
            {
                if (datetimePattern == formattedString)
                    return true;
            }

            return false;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        private OlapLabelPresenter CellDescriptor
        {
            get;
            set;
        }
        private OlapChart OlapChart
        {
            get;
            set;
        }
        #endregion
    }

    public class ExpanderStateConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ExpandableState state = (ExpandableState)value;
            return (state == ExpandableState.Expanded) ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

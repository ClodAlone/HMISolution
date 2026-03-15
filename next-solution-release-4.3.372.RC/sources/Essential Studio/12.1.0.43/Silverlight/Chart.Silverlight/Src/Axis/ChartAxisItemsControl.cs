#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Linq;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Globalization;
    using Syncfusion.Windows.Data;

    

    /// <summary>
    /// The ChartAxis class represents an axis of the <see
    /// cref="ChartArea">ChartArea</see>.
    /// </summary>
    /// <remarks>
    /// A ChartArea contains a minimum of two axes namely primary axis and secondary
    /// axis in a Chart control. Values / data in the chart are plotted against these
    /// axes. Chart can also supports for adding multiple axes to the chart area and the
    /// series can be drawn on any axis in the collection.
    /// </remarks>
    /// <example>
    /// XAML: 
    /// <code>&lt;syncfusion:Chart x:Name=&quot;chart&quot;&gt;
    /// &lt;syncfusion:ChartArea Name=&quot;area&quot;&gt;
    ///           &lt;syncfusion:ChartArea.PrimaryAxis&gt;
    ///               &lt;syncfusion:ChartAxis Header=&quot;X-Axis&quot;  /&gt;
    ///            &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
    ///            &lt;syncfusion:ChartArea.SecondaryAxis&gt;
    ///                &lt;syncfusion:ChartAxis Header=&quot;Y-Axis&quot; /&gt;
    ///            &lt;/syncfusion:ChartArea.SecondaryAxis&gt;
    ///            &lt;syncfusion:ChartSeries Name=&quot;series&quot; Data=&quot;1,6,2,5,3,6,4,10&quot; /&gt;                                     &lt;/syncfusion:ChartArea&gt;
    /// &lt;/syncfusion:Chart&gt;</code>
    /// <para> </para>
    /// <para> C#: </para>
    /// <para> </para>
    /// <code> Chart chart=new Chart();
    ///  ChartAxis axis = new ChartAxis();
    ///  axis.Header = &quot;X-Axis&quot;;
    ///  chart.Areas[0].PrimaryAxis = axis;</code>
    /// <para> </para>
    /// <code> ChartAxis yaxis = new ChartAxis();
    ///  yaxis.Header = &quot;Y-Axis&quot;;
    ///  chart.Areas[0].SecondaryAxis = yaxis;</code>
    /// </example>
    /// <seealso cref="ChartArea">ChartArea class specification</seealso>
    /// <seealso cref="ChartSeries">ChartSeries class specification</seealso>
    public class ChartAxis : ItemsControl,IDisposable
    {
        internal bool m_enableRangeSelection = false;
        internal bool m_enableRangeSelectionMouseOver = false;
        /// <summary>
        /// Idenfities EnableRangeSelection dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableRangeSelectionProperty =
            DependencyProperty.Register("EnableRangeSelection", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableRangeChanged)));

        /// <summary>
        /// Gets or sets the EnableRangeSelection. This is dependency property.
        /// </summary>
        /// <value>The boolean Type.</value>
        public bool EnableRangeSelection
        {
            get
            {
                return (bool)GetValue(EnableRangeSelectionProperty);
            }

            set
            {
                SetValue(EnableRangeSelectionProperty, value);
            }
        }

        /// <summary>
        /// Idenfities EnableRangeSelectionOnMouseOver dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableRangeSelectionOnMouseOverProperty =
            DependencyProperty.Register("EnableRangeSelectionOnMouseOver", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableRangeSelectionChanged)));

        /// <summary>
        /// Gets or sets the EnableRangeSelectionOnMouseOver. This is dependency property.
        /// </summary>
        /// <value>The boolean Type.</value>
        public bool EnableRangeSelectionOnMouseOver
        {
            get
            {
                return (bool)GetValue(EnableRangeSelectionOnMouseOverProperty);
            }

            set
            {
                SetValue(EnableRangeSelectionOnMouseOverProperty, value);
            }
        }

        /// <summary>
        /// Idenfities SelectedRange dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedRangeProperty =
            DependencyProperty.Register("SelectedRange", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(new DoubleRange(), new PropertyChangedCallback(OnSelectedRangeChanged)));

        /// <summary>
        /// Gets or sets the SelectedRange. This is dependency property.
        /// </summary>
        /// <value>The DoubleRange Type.</value>
        public DoubleRange SelectedRange
        {
            get
            {
                return (DoubleRange)GetValue(SelectedRangeProperty);
            }

            set
            {
                SetValue(SelectedRangeProperty, value);
            }
        }
        private readonly static double[] c_intervalDivs = new double[] { 1d, 2d, 3d, 5d };
      /// <summary>
        ///  Identifies the IsInversed dependency property.
      /// </summary>
        public static readonly DependencyProperty IsInversedProperty =
DependencyProperty.Register("IsInversed", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnIsInversedChanged)));

        /// <summary>
        /// Get or Set IsInversedProperty
        /// </summary>
        public bool IsInversed
        {
            get
            {
                return (bool)GetValue(IsInversedProperty);
            }

            set
            {
                SetValue(IsInversedProperty, value);
            }
        }
        private static void OnEnableRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (e.NewValue.ToString() == "False" && axis.ParentArea != null)
                {
                    axis.ParentArea.RangeSelectionHeight = 0d;
                    axis.ParentArea.RangeSelectionWidth = 0d;
                    axis.ParentArea.CloseButtonVisibility = Visibility.Collapsed;
                    axis.SelectedRange = new DoubleRange();
                }
            }
        }
        private static void OnEnableRangeSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (e.NewValue.ToString() == "False" && axis.ParentArea != null)
                {
                    axis.ParentArea.RangeSelectionMouseOverHeight = 0d;
                    axis.ParentArea.RangeSelectionMouseOverWidth = 0d;
                    axis.SelectedRange = new DoubleRange();
                }
                
            }
        }

        private static void OnSelectedRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.OnSelectedRangeChanged(e);
            }
        }
        /// <summary>
        /// Called when SelectionRange property changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedRangeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.SelectedRangeChanged != null)
            {
                this.SelectedRangeChanged(this, e);
            }
        }

        private static void OnIsInversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.axisElementPanel != null)
            {
                axis.axisElementPanel.InvalidateArrange();

                foreach (ChartSeries series in axis.axisBindedSeriesList)
                {
                    if (series.XAxis.Equals(axis))
                        series.IsXAxisInversed = axis.IsInversed;
                    else if(series.YAxis.Equals(axis))
                        series.IsYAxisInversed=axis.IsInversed;
                }
            }
            if(axis.Area != null)
            axis.Area.LoadArea();
        }

        #region revamp
        bool m_isTimeInterval = false;
        internal bool IsTimeInterval
        {
            get { return m_isTimeInterval; }
            set { m_isTimeInterval = value; }
        }
        /// <summary>
        ///  Identifies the LabelRotateTransform dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelRotateTransformProperty =
DependencyProperty.Register("LabelRotateTransform", typeof(TransformGroup), typeof(ChartAxis), new PropertyMetadata(new TransformGroup()));

        /// <summary>
        /// Get or Set LabelRotateTransformProperty
        /// </summary>
        public TransformGroup LabelRotateTransform
        {
            get { return (TransformGroup)GetValue(LabelRotateTransformProperty); }
            internal set { SetValue(LabelRotateTransformProperty, value); }
        }


        internal ObservableCollection<ChartSeries> axisBindedSeriesList = new ObservableCollection<ChartSeries>();
        /// <summary>
        /// Identifies the ActualVisibleRange dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualVisibleRangeProperty =
    DependencyProperty.Register("ActualVisibleRange", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(new DoubleRange(0, 1), new PropertyChangedCallback(OnActualVisibleRangeChanged)));

        /// <summary>
        /// Get or Set ActualVisibilityRange property
        /// </summary>
        public DoubleRange ActualVisibleRange
        {
            get { return (DoubleRange)GetValue(ActualVisibleRangeProperty); }
            internal set { SetValue(ActualVisibleRangeProperty, value); }
        }
        /// <summary>
        /// Identifies the ActualvisibleInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualVisibleIntervalProperty =
DependencyProperty.Register("ActualVisibleInterval", typeof(double), typeof(ChartAxis), new PropertyMetadata(1d, new PropertyChangedCallback(OnActualVisibleIntervalChanged)));
        /// <summary>
        /// Get or Set ActualVisibleIntervalProperty
        /// </summary>
        public double ActualVisibleInterval
        {
            get { return (double)GetValue(ActualVisibleIntervalProperty); }
            internal set { SetValue(ActualVisibleIntervalProperty, value); }
        }
        /// <summary>
        /// Identifies the ChartAxesProvider dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartAxesProviderProperty =
DependencyProperty.Register("ChartAxesProvider", typeof(IChartAxes), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnAxesProviderChanged)));
        /// <summary>
        /// Get or Set ChartaxesProviderProperty
        /// </summary>
        public IChartAxes ChartAxesProvider
        {
            get { return (IChartAxes)GetValue(ChartAxesProviderProperty); }
            set { SetValue(ChartAxesProviderProperty, value); }
        }

        internal event PropertyChangedCallback GridLinesChanged;

        internal static readonly DependencyProperty IsRefreshGridLinesProperty =
          DependencyProperty.Register("IsRefreshGridLines", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true, new PropertyChangedCallback(OnGridLineRefreshChanged)));

        internal bool IsRefreshGridLines
        {
            get { return (bool)GetValue(IsRefreshGridLinesProperty); }
            set { SetValue(IsRefreshGridLinesProperty, value); }
        }

        private static void OnGridLineRefreshChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnGridLineRefreshChanged(e);
        }
        /// <summary>
        /// Called when IsRefreshGridLines property changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnGridLineRefreshChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GridLinesChanged != null)
            {
                this.GridLinesChanged(this, e);
            }
        }

        ChartAxisModel m_Model = new ChartAxisModel();
        internal ChartAxisModel Model
        {
            get
            {
                return m_Model;
            }

            set
            {
                m_Model = value;
            }
        }

        internal bool IsRefreshItems
        {
            get;
            set;
        }

        internal double AxisLabelMaxSize
        {
            get;
            set;
        }

        internal void RefreshAxis()
        {
            this.IsRefreshItems = true;
            this.InvalidateMeasure();
        }

        internal void UpdateAxis()
        {
            //this.IsRefreshItems = false;
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Method implementation for axis visibility in chart
        /// </summary>
        public void GoToAxisVisualState()
        {
            GoToAxisLabelVisualState();
            if (this.ChartAxesProvider is IChartCartesianAxes)
            {
                if (this.Orientation == Orientation.Horizontal && this.OpposedPosition == false)
                {
                    VisualStateManager.GoToState(this, "AxisHorizontal_Bottom_State", true);
                }
                else if (this.Orientation == Orientation.Horizontal && this.OpposedPosition == true)
                {
                    VisualStateManager.GoToState(this, "AxisHorizontal_Top_State", true);
                }
                else if (this.Orientation == Orientation.Vertical && this.OpposedPosition == false)
                {
                    VisualStateManager.GoToState(this, "AxisVertical_Left_State", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "AxisVertical_Right_State", true);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "Non_CartesianAxes", true);
            }
        }

        /// <summary>
        /// Method implementation for Choose visualState manager based upon the axis orientation
        /// </summary>
        public void GoToAxisLabelVisualState()
        {
            if (this.Items == null)
                return;

            foreach (Control control in this.Items)
            {
                if (!(this.ChartAxesProvider is IChartCartesianAxes) && this.Orientation==Orientation.Horizontal)
                {
                    VisualStateManager.GoToState(control, "Non_Cartesian_Axis_State", true);
                }
                else if (this.Orientation == Orientation.Horizontal && this.OpposedPosition == false)
                {
                    VisualStateManager.GoToState(control, "Horizontal_NotOpposed_State", true);
                }
                else if (this.Orientation == Orientation.Horizontal && this.OpposedPosition == true)
                {
                    VisualStateManager.GoToState(control, "Horizontal_Opposed_State", true);
                }
                else if (this.Orientation == Orientation.Vertical && this.OpposedPosition == false)
                {
                    VisualStateManager.GoToState(control, "Vertical_NotOpposed_State", true);
                }
                else
                {
                    VisualStateManager.GoToState(control, "Vertical_Opposed_State", true);
                }
            }
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = new Size(double.IsInfinity(availableSize.Width) ? 300 : availableSize.Width, double.IsInfinity(availableSize.Height) ? 800 : availableSize.Height);
            this.Model.TotalRenderSize = availableSize;
            //IsRefreshItems = true;
            if (IsRefreshItems)
            {
                GenerateAxisLabel(availableSize);
            }
            else
            {
                UpdateItems(availableSize);
            }
            this.IsRefreshGridLines = !this.IsRefreshGridLines;
            
            this.IsRefreshItems = false;

            return base.MeasureOverride(availableSize);
        }


        /// <summary>
        /// Method implementation for Updateitems in ChartAxis
        /// </summary>
        /// <param name="avilabelSize"></param>
        public void UpdateItems(Size avilabelSize)
        {
            int countdata = 0;
            int countdata1 = 0;
            int countindexdata = 0;
            FirstInterval();
            if (this.LabelsSource != null)
                this.IndexDataContents = DataBinding.GetOrderedContentData(this);

            Size TotalSize = new Size();
            TotalSize = new Size(double.IsNaN(avilabelSize.Width) ? 0 : avilabelSize.Width, double.IsNaN(avilabelSize.Height) ? 0 : avilabelSize.Height);
            if (this.ChartAxesProvider == null)
                return;

            int index = 0;
            this.Items.Clear();

            List<double> anglepoints = this.Area != null && this.Area.PrimaryAxis != null && this.ChartAxesProvider is IChartRadarAxes ? (from label in this.Area.PrimaryAxis.Items.OfType<DataAxis>() select label.PointInfo.Angle).Cast<double>().ToList() : new List<double>();

            bool isOpposedPosition = this.OpposedPosition;
            if (this.ChartAxesProvider is IChartRadarAxes && this.Area != null)
            {
                isOpposedPosition = ChartRadarType.GetIsClockWise(this.Area);
            }
            else if (this.Area != null && this.ChartAxesProvider is IChartPolarAxes)
            {
                isOpposedPosition = ChartPolarType.GetIsClockWise(this.Area);
            }
            IEnumerable<ChartAxisPoints> PointCollection = this.ChartAxesProvider.GetPoints(this.ActualVisibleRange.Start, this.ActualVisibleRange.End, this.ActualVisibleInterval, this.Orientation, isOpposedPosition, TotalSize, anglepoints, this.IsLogarithmic, this.LogarithmicBase, this.firstinterval, this.m_enableBreaks, this);
            if (this._IsSortedSeries)
                PointCollection = this.ChartAxesProvider.GetSortedPoints(this.ActualVisibleRange.Start, this.ActualVisibleRange.End, this.ActualVisibleInterval, this.Orientation, isOpposedPosition, TotalSize, anglepoints, this.IsLogarithmic, this.LogarithmicBase, this.firstinterval, this);
            IEnumerable<ChartAxisPoints> sortedLabels = PointCollection;
            DataAxis tempaxis = null;
           
            foreach (ChartAxisPoints point in sortedLabels)
            {
                DataAxis linePoint = getDataAxisObject(index, point.ActualValue == this.ActualVisibleRange.End);// new DataAxis();
                linePoint.HorizontalAlignment = HorizontalAlignment.Left;
                linePoint.VerticalAlignment = VerticalAlignment.Top;
                linePoint.RelatedAxis = this;

                linePoint.ActualValue = point.ActualValue;
                linePoint.X1 = point.X1;
                linePoint.Y1 = point.Y1;
                linePoint.X2 = point.X2;
                linePoint.Y2 = point.Y2;
                linePoint.PointInfo = point;
                if (this.ChartAxesProvider is IChartRadarAxes)
                {
                    linePoint.PolyPoints = point.PolyPoints;
                    if (this.Area != null)
                    {
                        this.Area.centerPoint = point.CenterPoint;
                        this.Area.angle = this.Orientation == Orientation.Horizontal ? point.Angle : this.Area.angle;
                        this.Area.maxRadius = point.MaxRadius;
                    }
                }
                else if (this.ChartAxesProvider is IChartPolarAxes)
                {
                    linePoint.Radius = point.Radius;
                    if (this.Area != null)
                    {
                        this.Area.centerPoint = point.CenterPoint;
                        this.Area.angle = this.Orientation == Orientation.Horizontal ? point.Angle : this.Area.angle;
                        this.Area.maxRadius = point.MaxRadius;
                    }
                }

                linePoint.LabelPosition = new Thickness(this.Orientation == Orientation.Horizontal ? linePoint.AxisMargin.Left : 0, linePoint.AxisMargin.Top, linePoint.AxisMargin.Right, linePoint.AxisMargin.Bottom);

                #region getLabels values
                if (this.LabelsSource != null && (this.AxisType == typeof(string) || this.ContentPath != this.PositionPath))
                {
                    if (this.Isindexedseries == false && IndexDataContents != null)
                    {
                        if (countdata < this.IndexDataContents.Count)
                        {
                            //if (axis.IsLogarithmic && axis.IsLogarithmicLabels)
                            //{
                            //    axisValues.Content = axis.ValueToLogValue(double.Parse(axis.IndexDataContents[countdata].ToString()));
                            //}
                            //else
                            linePoint.Label = this.IndexDataContents[countdata].ToString();
                        }
                        else
                        {
                            linePoint.Label = "";
                        }
                    }
                    else
                    {
                        if (AxisDataContents != null)
                        {
                            countindexdata = Convert.ToInt32(Math.Floor(point.ActualValue));
                            if (countdata1 > -1 && countdata1 < this.AxisDataContents.Count && this.ValueType == ChartValueType.DateTime)
                            {
                                linePoint.Label = this.AxisDataContents[countdata1].ToString();
                            }
                            else if (countindexdata > -1 && countindexdata < this.AxisDataContents.Count && this.ValueType != ChartValueType.DateTime)
                            {
                                linePoint.Label = this.AxisDataContents[countindexdata].ToString();
                            }
                            else
                            {
                                linePoint.Label = "";
                            }
                        }
                    }

                    countdata1 += Convert.ToInt32(this.VisibleInterval);
                    countdata++;
                }
                else if (this.Isindexedseries == true)
                {
                    countindexdata = Convert.ToInt32(Math.Floor(point.ActualValue));
                    if (countindexdata > -1 && countindexdata < this.IndexDataContents.Count)
                    {
                        //if (this.IsLogarithmic)
                        //{
                        //    linePoint.Label = this.IndexDataContents[countindexdata].ToString();
                        //    if (this.IndexDataContents[countindexdata].ToString() != "")
                        //        linePoint.Label = this.ValueToLogValue(double.Parse(this.IndexDataContents[countindexdata].ToString()));
                        //}
                        //else
                        ChartPointsCollection indexedpoints = new ChartPointsCollection();
                        if (this._IsSortedSeries)
                        {
                            Comparer comparer = new Comparer(new CultureInfo("en-US"));
                            for (int i = 0; i < this.SeriesData.Count; i++)
                            {
                                if (i + 1 < this.IndexDataContents.Count && this.IndexDataContents[i + 1].ToString() != "")
                                    indexedpoints.Add(new ChartPoint(double.Parse(this.IndexDataContents[i + 1].ToString()), this.SeriesData[i].Y));
                            }
                            for (int i = 0; i < indexedpoints.Count - 1; i++)
                            {
                                for (int j = i; j <= indexedpoints.Count - 1; j++)
                                {
                                    var compare = comparer.Compare(indexedpoints[i].Y, indexedpoints[j].Y);
                                    if (this._SortingDirection == Direction.Ascending ? compare > 0 : compare < 0)
                                    {
                                        var temp = indexedpoints[i];
                                        indexedpoints[i] = indexedpoints[j];
                                        indexedpoints[j] = temp;
                                    }
                                }
                            }

                            indexedpoints.Insert(0, new ChartPoint(0, 0));
                            indexedpoints.Insert(indexedpoints.Count, new ChartPoint(0, 0));
                            linePoint.Label = countindexdata == 0 || countindexdata == indexedpoints.Count - 1 ? "" : countindexdata < indexedpoints.Count ?indexedpoints[countindexdata].X.ToString():"";
                        }
                        else
                        {
                            linePoint.Label = this.IndexDataContents[countindexdata].ToString();
                        }
                    }
                    else
                    {
                        linePoint.Label = "";
                    }
                }
                else
                {
                    linePoint.Label = (this.VisibleInterval == this.firstinterval && Math.Round(this.VisibleInterval, 10) != Math.Round(this.firstinterval, 10)) ? "" : point.ActualValue.ToString();
                    if (this.IsLogarithmic && this.LogarithmicBase == 10)
                        linePoint.Label = Math.Ceiling(point.ActualValue).ToString();
                }

                string formattedcontent = this.GetFormattedAxisLabel(linePoint.Label.ToString());
                linePoint.Label = formattedcontent != null ? formattedcontent : linePoint.Label;
                if ((this.Prefix != string.Empty || this.Suffix != string.Empty) && !linePoint.Label.ToString().Equals(""))
                {
                    linePoint.Label = this.Prefix + linePoint.Label.ToString() + this.Suffix;
                }
                #endregion

                if (!this.Items.Contains(linePoint))
                {
                    if (tempaxis != null && tempaxis.RelatedAxis.ChartAxesProvider is IChartCartesianAxes && !m_enableBreaks)
                    {
                        if (tempaxis.AxisMargin != linePoint.AxisMargin)
                        {
                            this.Items.Add(linePoint);
                        }
                    }
                    else
                    {
                        this.Items.Add(linePoint);
                    }
                    tempaxis = linePoint;
                }

                index++;
            }
        }


        DataAxis getDataAxisObject(int index, bool isLast)
        {
            if (isLast)
            {
                int val = this.Items.Count;
                for (int i = index + 1; i < val; i++)
                {
                    if (i < this.Items.Count)
                    {
                        this.Items.RemoveAt(i);
                    }
                }
            }

            if (this.Items.Count > index)
            {
                DataAxis result = this.Items[index] as DataAxis;
                if (result != null)
                {
                    result.Visibility = Visibility.Visible;
                    return result;
                }
            }

            return new DataAxis();
        }

        internal void FirstInterval()
        {
            if (this.AxisType != typeof(string) && this.ContentPath == this.PositionPath)
            {
                this.firstinterval = double.NaN;
                return;
            }

            double diff = Math.Ceiling(this.ActualVisibleRange.Start) - this.ActualVisibleRange.Start;
            if (this.AxisType != typeof(DateTime) && (this.IsFractionEnabledOnZoom && this.LabelsSource != null) && diff > 0)
            {
                this.firstinterval = diff;
            }
            else if (diff == 0.0)
            {
                this.firstinterval = double.NaN;
            }
            else if (this.ValueType == ChartValueType.DateTime && this.ZoomFactor < 1)
            {
                DateTime datetimeinterval = DateTime.FromOADate(this.VisibleInterval);
                double dateinterval = Math.Floor(datetimeinterval.ToOADate());
                double timeinterval = datetimeinterval.ToOADate() - Math.Floor(datetimeinterval.ToOADate());
                if (dateinterval == 0 && timeinterval > 0 && timeinterval < 1)
                {
                    long ticks = this.DateTimeInterval.Ticks - (DateTime.FromOADate(this.Range.Start).TimeOfDay.Ticks % this.DateTimeInterval.Ticks);
                    this.firstinterval = new DateTime(ticks).ToOADate();
                }
                else
                {
                    double days = this.ActualVisibleInterval - (this.ActualVisibleRange.Start % this.VisibleInterval);
                    this.firstinterval = days <= 0d ? 1 : days;
                }
            }
        }

        /// <summary>
        /// Method implementation for creating axis labels 
        /// </summary>
        /// <param name="avilabelSize"></param>
        public void GenerateAxisLabel(Size avilabelSize)
        {
            double SegmentMinValue = this.ActualVisibleRange.Start + (this.ActualVisibleRange.Start * (-1));
            double SegmentMaxValue = this.ActualVisibleRange.End - this.ActualVisibleRange.Start;
            SegmentMaxValue = (SegmentMaxValue == 0) ? 1 : SegmentMaxValue;
            FirstInterval();

            if (this.LabelsSource != null && this.ContentPath != null)
            {
                //this.AxisType = DataBinding.GetPropertyType(this.LabelsSource, this.ContentPath);
                this.IndexDataContents = DataBinding.GetOrderedContentData(this);
            }
            
            

           // this.AxisLines.Clear();
            int countdata = 0;
            int countdata1 = 0;
            int countindexdata = 0;
            int index = 0;

            this.Items.Clear();

            if (this.ChartAxesProvider == null && this.Area != null && this.Area.AreaType == ChartAxesType.CartesianAxes)
            {
                this.ChartAxesProvider = new ChartCartesianAxesGenerator();
            }
            else if (this.ChartAxesProvider == null)
            {
                return;
            }

            Size TotalSize = new Size();
            TotalSize = new Size(double.IsNaN(avilabelSize.Width) ? 0 : avilabelSize.Width, double.IsNaN(avilabelSize.Height) ? 0 : avilabelSize.Height);
            List<double> anglepoints = this.Area != null && this.Area.PrimaryAxis != null && this.ChartAxesProvider is IChartRadarAxes ? (from label in this.Area.PrimaryAxis.Items.OfType<DataAxis>() select label.PointInfo.Angle).Cast<double>().ToList() : new List<double>();

            bool isOpposedPosition = this.OpposedPosition;
            if (this.ChartAxesProvider is IChartRadarAxes && this.Area!=null)
            {
                isOpposedPosition = ChartRadarType.GetIsClockWise(this.Area);
            }
            else if (this.Area != null && this.ChartAxesProvider is IChartPolarAxes)
            {
                isOpposedPosition = ChartPolarType.GetIsClockWise(this.Area);
            }
            IEnumerable<ChartAxisPoints> PointCollection = this.ChartAxesProvider.GetPoints(this.ActualVisibleRange.Start, this.ActualVisibleRange.End, this.ActualVisibleInterval, this.Orientation, isOpposedPosition, TotalSize, anglepoints, this.IsLogarithmic, this.LogarithmicBase, this.firstinterval, this.m_enableBreaks, this);
            if(this._IsSortedSeries)
                PointCollection = this.ChartAxesProvider.GetSortedPoints(this.ActualVisibleRange.Start, this.ActualVisibleRange.End, this.ActualVisibleInterval, this.Orientation, isOpposedPosition, TotalSize, anglepoints, this.IsLogarithmic, this.LogarithmicBase, this.firstinterval,this);
            IEnumerable<ChartAxisPoints> sortedLabels = PointCollection;
             DataAxis tempaxis=null;
            foreach (ChartAxisPoints point in PointCollection)
            {
                DataAxis linePoint = getDataAxisObject(index, point.ActualValue == this.ActualVisibleRange.End);// new DataAxis();
                linePoint.HorizontalAlignment = HorizontalAlignment.Left;
                linePoint.VerticalAlignment = VerticalAlignment.Top;
                linePoint.RelatedAxis = this;
                linePoint.ActualValue = point.ActualValue;
                linePoint.X1 = point.X1;
                linePoint.Y1 = point.Y1;
                linePoint.X2 = point.X2;
                linePoint.Y2 = point.Y2;
                linePoint.PointInfo = point;
                if (this.ChartAxesProvider is IChartRadarAxes)
                {
                    linePoint.PolyPoints = point.PolyPoints;
                    if (this.Area != null)
                    {
                        this.Area.centerPoint = point.CenterPoint;
                        this.Area.angle = this.Orientation == Orientation.Horizontal ? point.Angle : this.Area.angle;
                        this.Area.maxRadius = point.MaxRadius;
                    }
                }
                else if (this.ChartAxesProvider is IChartPolarAxes)
                {
                    linePoint.Radius = point.Radius;
                    if (this.Area != null)
                    {
                        this.Area.centerPoint = point.CenterPoint;
                        this.Area.angle = this.Orientation == Orientation.Horizontal ? point.Angle : this.Area.angle;
                        this.Area.maxRadius = point.MaxRadius;
                    }
                }

                #region getLabels values
                if (this.LabelsSource != null && (this.AxisType == typeof(string) || this.ContentPath != this.PositionPath))
                {
                    if (this.Isindexedseries == false && this.IndexDataContents != null)
                    {
                        if (countdata >= 0 && countdata < this.IndexDataContents.Count)
                        {
                            linePoint.Label = this.IndexDataContents[countdata].ToString();
                        }
                        else
                        {
                            linePoint.Label = "";
                        }
                    }
                    else
                    {
                        countindexdata = Convert.ToInt32(Math.Floor(point.ActualValue));
                        //if (countdata1 < this.AxisDataContents.Count && this.ValueType == ChartValueType.DateTime)
                        //{
                        //    linePoint.Label = this.AxisDataContents[countdata1].ToString();
                        //}
                        if (this.AxisDataContents != null && countindexdata < this.AxisDataContents.Count)// && this.ValueType != ChartValueType.DateTime)
                        {
                            linePoint.Label = this.AxisDataContents[countindexdata].ToString();
                        }
                        else
                        {
                            linePoint.Label = "";
                        }
                    }

                    countdata1 += Convert.ToInt32(this.VisibleInterval);
                    countdata++;
                }
                else if (this.Isindexedseries == true)
                {
                    countindexdata = Convert.ToInt32(Math.Floor(point.ActualValue));
                    if (countindexdata > -1 && countindexdata < this.IndexDataContents.Count)
                    {
                        linePoint.Label = this.IndexDataContents[countindexdata].ToString();
                    }
                    else
                    {
                        linePoint.Label = "";
                    }
                }
                else
                {
                    linePoint.Label = (this.VisibleInterval == this.firstinterval && Math.Round(this.VisibleInterval, 10) != Math.Round(this.firstinterval, 10)) ? "" : point.ActualValue.ToString();
                    if (this.IsLogarithmic && this.LogarithmicBase == 10)
                        linePoint.Label = Math.Ceiling(point.ActualValue).ToString();
                }

                string formattedcontent = this.GetFormattedAxisLabel(linePoint.Label.ToString());
                linePoint.Label = formattedcontent != null ? formattedcontent : linePoint.Label;
                if ((this.Prefix != string.Empty || this.Suffix != string.Empty) && !linePoint.Label.ToString().Equals(""))
                {
                    linePoint.Label = this.Prefix + linePoint.Label.ToString() + this.Suffix;
                }
                #endregion

                if (!this.Items.Contains(linePoint))
                {
                    if (tempaxis != null && tempaxis.RelatedAxis.ChartAxesProvider is IChartCartesianAxes && !m_enableBreaks)
                    {
                        if (tempaxis.AxisMargin != linePoint.AxisMargin)
                        {
                            this.Items.Add(linePoint);
                        }
                    }
                    else
                    {
                        this.Items.Add(linePoint);
                    }
                   tempaxis=linePoint;
                }

                index++;
            }
        }

        /// <summary>
        /// Prepares the specified element to display the specified item. 
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param><param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            DataAxis dataAxis = element as DataAxis;
            if (dataAxis != null && !double.IsNaN(dataAxis.AxisMargin.Left) && !double.IsNaN(dataAxis.AxisMargin.Top))
            {
                dataAxis.LabelPosition = new Thickness(this.Orientation == Orientation.Horizontal ? dataAxis.AxisMargin.Left : 0, dataAxis.AxisMargin.Top, dataAxis.AxisMargin.Right, dataAxis.AxisMargin.Bottom);
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        #endregion

        internal double ZoomInterval = 0d, ZoomVisisbleInterval=0d, ActualInterval, ActualZoomPosition = double.NaN;
        internal bool isUpdateZoomrange = false, zoomisautosetrange = false;
        internal bool isUpdateViewportsize = false, isUpdateScrollbar = false;
        internal bool isZoomfactorincreased = false;
        internal bool isApplyIntersectAction = false;
        internal double maxrowsize = 0d;
        internal double rotatemarginvalue = 0d;
        internal double firstinterval = double.NaN;
        internal double startpadding = 0d, endpadding = 0d, rotateactionangle = 0d;
        internal bool isRotatedAxis = false;
        private Type m_type = typeof(double);
        private ChartArea m_area = null;
        internal double lastPosition_X = 0d;
        internal double lasPosition_Y = 0d;
        private ChartAxisLabelsCollection m_customLables = new ChartAxisLabelsCollection();
        private IEnumerable<ChartAxisLabel> m_labelSourceLables = new ChartAxisLabelsCollection();
        private bool m_isActualLabelsSourceSet = false, m_internalLabelsSourceSet = false;
        internal bool isIntervalSet = false;
        internal bool InternalLabelsSourceSet
        {
            get
            {
                return m_internalLabelsSourceSet;
            }
            set
            {
                m_internalLabelsSourceSet = value;
            }
        }

        internal bool ActualLabelsSourceSet
        {
            get
            {
                return m_isActualLabelsSourceSet;
            }
            set
            {
                m_isActualLabelsSourceSet = value;
            }
        }

        internal IEnumerable<ChartAxisLabel> ActualLabels
        {
            get
            {
                return m_labelSourceLables.Union<ChartAxisLabel>(m_customLables);
            }
            set
            {
                m_labelSourceLables = value;
            }

        }

        internal ChartArea ParentArea
        {
            get;
            set;
        }

        internal DoubleRange ZoomRange
        {
            get;
            set;
        }

        internal DoubleRange ZoomVisibleRange
        {
            get;
            set;
        }

        internal DoubleRange PreviousRange
        {
            get;
            set;
        }

        internal Type AxisType
        {
            get
            {
                return m_type;
            }

            set
            {
                m_type = value;
            }
        }

        private ChartStripLinesCollection m_stripLines = new ChartStripLinesCollection();

        /// <summary>
        /// get or Set  StripLines property
        /// </summary>
        public ChartStripLinesCollection StripLines
        {
            get
            {
                return m_stripLines;
            }
        }
        /// <summary>
        ///  Identifies the LograthimicRange dependency property.
        /// </summary>
        public static readonly DependencyProperty LogarithmicRangeProperty =
DependencyProperty.Register("LogarithmicRange", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(DoubleRange.Empty, new PropertyChangedCallback(OnLogRangeChanged)));
        /// <summary>
        /// Get or Set LogarithmicRangeProperty
        /// </summary>
        public DoubleRange LogarithmicRange
        {
            get { return (DoubleRange)GetValue(LogarithmicRangeProperty); }
            set { SetValue(LogarithmicRangeProperty, value); }
        }

        /// <summary>
        /// Identifies the Origin dependency property.
        /// </summary>
        public static readonly DependencyProperty OriginProperty =
          DependencyProperty.Register("Origin", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d, new PropertyChangedCallback(OnOriginPropertyChanged)));
        /// <summary>
        /// Called when OriginProperty Changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnOriginPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = obj as ChartAxis;
            if (axis.Area != null)
                axis.Area.LoadArea();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this axis is Origin.
        /// </summary>
        public double Origin
        {
            set { SetValue(OriginProperty, value); }
            get { return (double)GetValue(OriginProperty); }
        }

        /// <summary>
        /// Identifies the IsLogarithmic dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLogarithmicProperty =
          DependencyProperty.Register("IsLogarithmic", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnIsLogarithmicPropertyChanged)));

        /// <summary>
        /// Called when IsLogarithmicProperty is changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsLogarithmicPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = obj as ChartAxis;
            if (axis.Area != null)
                axis.Area.LoadArea();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this axis is logarithmic.
        /// </summary>
        public bool IsLogarithmic
        {
            set { SetValue(IsLogarithmicProperty, value); }
            get { return (bool)GetValue(IsLogarithmicProperty); }
        }

        /// <summary>
        /// Identifies the LogarithmicBase dependency property.
        /// </summary>
        public static readonly DependencyProperty LogarithmicBaseProperty =
            DependencyProperty.Register("LogarithmicBase", typeof(double), typeof(ChartAxis), new PropertyMetadata(10d, OnLogarithmicBasePropertyChanged));
        /// <summary>
        /// Called when LogarithmicBaseProperty Changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnLogarithmicBasePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis axis = obj as ChartAxis;
            if (axis.Area != null)
                if (axis.IsLogarithmic)
                    axis.Area.LoadArea();
        }

        /// <summary>
        /// Gets or sets the logarithmic base.
        /// </summary>
        /// <value>The logarithmic base.</value>
        public double LogarithmicBase
        {
            set { SetValue(LogarithmicBaseProperty, value); }
            get { return (double)GetValue(LogarithmicBaseProperty); }
        }

        internal double ValueToLogValue(double value)
        {
            double result = double.NaN;
            if (value > 0)
            {
                result = Math.Log(value, LogarithmicBase); 
                return result;
            }

            return result;
        }

        /// <summary>
        /// Header of the ChartAxis Depedency Property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ChartAxis), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Header for chart axis
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// HeaderTemplate of the Chart Axis Depedency Property
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the HeaderTemplate for chart axis
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the DateTimeInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty IgnoreRangePaddingsOnZoomProperty =
            DependencyProperty.Register("IgnoreRangePaddingsOnZoom", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnApperanceChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether range paddings should be ignored when axis is zoomed.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if range paddings are ignored on zoom; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreRangePaddingsOnZoom
        {
            get { return (bool)GetValue(IgnoreRangePaddingsOnZoomProperty); }
            set { SetValue(IgnoreRangePaddingsOnZoomProperty, value); }
        }

        /// <summary>
        /// Identifies the RangePadding dependency property.
        /// </summary>
        public static readonly DependencyProperty RangePaddingProperty =
          DependencyProperty.Register("RangePadding", typeof(ChartRangePaddingType), typeof(ChartAxis), new PropertyMetadata(ChartRangePaddingType.Normal, new PropertyChangedCallback(OnRangePaddingChanged)));


        /// <summary>
        /// Get or Set AdditionalPaddingProperty
        /// </summary>
        public DoubleRange AdditionalPadding
        {
            get { return (DoubleRange)GetValue(AdditionalPaddingProperty); }
            set { SetValue(AdditionalPaddingProperty, value);}
        }

        
        /// <summary>
        ///Using a DependencyProperty as the backing store for AdditionalPadding.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty AdditionalPaddingProperty =
            DependencyProperty.Register("AdditionalPadding", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(new DoubleRange(3,1,false)));

        



        /// <summary>
        /// Gets or sets the range padding for axis' range.
        /// </summary>
        /// <value>The value should be set from one of <see cref="ChartRangePaddingType"/> enumeration.</value>
        public ChartRangePaddingType RangePadding
        {
            get { return (ChartRangePaddingType)GetValue(RangePaddingProperty); }
            set { SetValue(RangePaddingProperty, value); }
        }

        /// <summary>
        /// Identifies the RangeCalculationMode dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeCalculationModeProperty =
     DependencyProperty.Register("RangeCalculationMode", typeof(RangeCalculationMode), typeof(ChartAxis), new PropertyMetadata(RangeCalculationMode.AdjustAcrossChartTypes, new PropertyChangedCallback(OnRangeCalculationModeChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether the series PrimaryAxis range should be consistent for all chart types.
        /// </summary>
        public RangeCalculationMode RangeCalculationMode
        {
            get { return (RangeCalculationMode)GetValue(RangeCalculationModeProperty); }
            set { SetValue(RangeCalculationModeProperty, value); }
        }

        /// <summary>
        /// Identifies the EdgeLabelsDrawingMode dependency property.
        /// </summary>
        public static readonly DependencyProperty EdgeLabelsDrawingModeProperty =
            DependencyProperty.Register("EdgeLabelsDrawingMode", typeof(EdgeLabelsDrawingMode), typeof(ChartAxis), new PropertyMetadata(EdgeLabelsDrawingMode.Center, new PropertyChangedCallback(OnApperanceChanged)));

        /// <summary>
        /// Gets or sets a value indicating mode that controls partially visible labels
        /// behaviour.
        /// </summary>
        /// <remarks>
        /// This property is used to set the drawing option for the edge labels, which are
        /// render outside of an chart area.
        /// </remarks>
        /// <value>
        /// <c>true</c> if partial labels should be hidden; otherwise, <c>false</c>.
        /// </value>
        public EdgeLabelsDrawingMode EdgeLabelsDrawingMode
        {
            get { return (EdgeLabelsDrawingMode)GetValue(EdgeLabelsDrawingModeProperty); }
            set { SetValue(EdgeLabelsDrawingModeProperty, value); }
        }

        /// <summary>
        /// Identifies the HidePartialLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty HidePartialLabelProperty =
            DependencyProperty.Register("HidePartialLabel", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnApperanceChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether labels that appear partially should be
        /// hidden.
        /// </summary>
        /// <remarks>
        /// Use to determine the Labels visibility, when part of axis label is drawn outside
        /// of an chart area.
        /// </remarks>
        /// <value>
        /// <c>true</c> if partial labels should be hidden; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// XAML 
        /// <para> </para>
        /// <para> &lt;syncfusion:Chart x:Name=&quot;chart&quot;&gt;</para>
        /// <para> &lt;syncfusion:ChartArea&gt;</para>
        /// <para> &lt;syncfusion:ChartArea.PrimaryAxis&gt;</para>
        /// <para> &lt;syncfusion:ChartAxis HidePartialLabel=&quot;True&quot;/&gt;</para>
        /// <para> &lt;/syncfusion:ChartArea.PrimaryAxis&gt;</para>
        /// <para> &lt;/syncfusion:ChartArea&gt;</para>
        /// <para> &lt;/syncfusion:Chart&gt;</para>
        /// <para> </para>
        /// <para>C#</para>
        /// <para> </para>
        /// <para> ChartAxis axis = new ChartAxis();</para>
        /// <para> axis.HidePartialLabel = true;</para>
        /// </example>
        public bool HidePartialLabel
        {
            get { return (bool)GetValue(HidePartialLabelProperty); }
            set { SetValue(HidePartialLabelProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelRotateAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelRotateAngleProperty =
          DependencyProperty.Register("LabelRotateAngle", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d, new PropertyChangedCallback(OnRotateLabelChanged)));

        /// <summary>
        /// Gets or sets axis label rotation angle.
        /// </summary>
        /// <value>
        /// The Angle Rotate value in double
        /// </value>
        /// <example>
        /// [XAML]
        /// <para></para>
        /// <para> &lt;syncfusion:Chart x:Name=&quot;chart&quot;&gt;</para>
        /// <para>            &lt;syncfusion:ChartArea&gt;</para>
        /// <para>                &lt;syncfusion:ChartArea.PrimaryAxis&gt;</para>
        /// <para>                    &lt;syncfusion:ChartAxis
        /// IsAutoSetRange=&quot;False&quot; Range=&quot;0,10&quot;
        /// LabelRotateAngle=&quot;180&quot;/&gt;</para>
        /// <para>                &lt;/syncfusion:ChartArea.PrimaryAxis&gt; </para>
        /// <para>            &lt;/syncfusion:ChartArea&gt;</para>
        /// <para>        &lt;/syncfusion:Chart&gt;</para>
        /// <para></para>
        /// <para>[C#]</para>
        /// <para></para>
        /// <para>            ChartAxis axis = new ChartAxis();</para>
        /// <para>            axis.LabelRotateAngle = 180;</para>
        /// </example>
        public double LabelRotateAngle
        {
            get { return (double)GetValue(LabelRotateAngleProperty); }
            set { SetValue(LabelRotateAngleProperty, value); }
        }

        /// <summary>
        /// Identifies the IntersectAction dependency property.
        /// </summary>
        public static readonly DependencyProperty IntersectActionProperty =
          DependencyProperty.Register("IntersectAction", typeof(ChartLabelIntersectAction), typeof(ChartAxis), new PropertyMetadata(ChartLabelIntersectAction.None, new PropertyChangedCallback(OnIntersectActionChanged)));

        /// <summary>
        /// Gets or sets the intersecting layout behaviour for the labels of axis. This is a
        /// dependency property.
        /// </summary>
        /// <remarks>
        /// The overlapping between two or more axis labels can be avoided by using the
        /// IntersectAction property options. such as Multiple Rows, Rotate, Hide, Wrap and
        /// None.
        /// </remarks>
        /// <value>
        /// ChartLabelIntersectAction type value
        /// </value>
        public ChartLabelIntersectAction IntersectAction
        {
            get
            {
                return (ChartLabelIntersectAction)GetValue(IntersectActionProperty);
            }

            set
            {
                SetValue(IntersectActionProperty, value);
            }
        }

        /// <summary>
        /// Identifies the LabelTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
          DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the labels template. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// User can written own template design for chart axis labels.
        /// </remarks>
        /// <value>
        /// The DataTemplate value.
        /// </value>
        /// <example>
        /// C#: 
        /// <para> </para>
        /// <para><c>This property is not intended to be used from C#.</c></para>
        /// <para> </para>
        /// <para> XAML: </para>
        /// <para> </para>
        /// <code>&lt;UserControl xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
        /// xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
        /// Height=&quot;300&quot; Width=&quot;300&quot;&gt;
        /// &lt;xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight&quot;&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Type=&quot;Column&quot;
        /// Data=&quot;1,5,2,7,3,4,4,8,5,3&quot;/&gt;
        /// &lt;syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;syncfusion:ChartAxis&gt;
        /// &lt;syncfusion:ChartAxis.LabelTemplate&gt;
        /// &lt;DataTemplate&gt;
        /// &lt;Border&gt;
        /// &lt;Button Content=&quot;{Binding Path=Content, RelativeSource={RelativeSource TemplatedParent}}&quot;/&gt;
        /// &lt;/Border&gt;
        /// &lt;/DataTemplate&gt;
        /// &lt;/syncfusion:ChartAxis.LabelTemplate&gt;
        /// &lt;/syncfusion:ChartAxis&gt;
        /// &lt;/syncfusion:ChartArea.PrimaryAxis&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/UserControl&gt;</code>
        /// </example>
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        /// <summary>
        /// Idenfities IsFractionalData dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFractionalDataProperty =
            DependencyProperty.Register("IsFractionalData", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether IsFractionalData is true or
        /// false. This is dependency property.
        /// </summary>
        /// <remarks>
        /// This propert is to determine wheather the fractional values of chart axis to be
        /// include fractional range values on its axis
        /// </remarks>
        /// <value>
        /// true or false
        /// </value>
        public bool IsFractionalData
        {
            get
            {
                return (bool)GetValue(IsFractionalDataProperty);
            }

            set
            {
                SetValue(IsFractionalDataProperty, value);
            }
        }

        /// <summary>
        /// Idenfities IsFractionEnabledOnZoom dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFractionEnabledOnZoomProperty =
            DependencyProperty.Register("IsFractionEnabledOnZoom", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether IsFractionEnabledOnZoom is true or
        /// false. This is dependency property.
        /// </summary>
        /// <remarks>
        /// This propert is to determine wheather the fractional values of chart axis to be
        /// display or not on the time of perform zooming operation.
        /// </remarks>
        /// <value>
        /// true or false
        /// </value>
        public bool IsFractionEnabledOnZoom
        {
            get
            {
                return (bool)GetValue(IsFractionEnabledOnZoomProperty);
            }

            set
            {
                SetValue(IsFractionEnabledOnZoomProperty, value);
            }
        }

        /// <summary>
        /// Idenfities EnableZooming dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableZoomingProperty =
            DependencyProperty.Register("EnableZooming", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableZoomingChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether EnableZooming. This is dependency
        /// property.
        /// </summary>
        /// <remarks>
        /// Use to determine the Zooming functionalities to be enable on particular axis or
        /// not
        /// </remarks>
        /// <value>
        /// The Bool type.
        /// </value>
        public bool EnableZooming
        {
            get
            {
                return (bool)GetValue(EnableZoomingProperty);
            }

            set
            {
                SetValue(EnableZoomingProperty, value);
            }
        }

        /// <summary>
        /// Idenfities ZoomPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register("ZoomPosition", typeof(double), typeof(ChartAxis), new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnZoomPositionChanged)));

        /// <summary>
        /// Gets or sets the ZoomPosition. This is dependency property.
        /// </summary>
        /// <remarks>
        /// Set the position of axis to perform zooming operations.
        /// </remarks>
        /// <value>
        /// The Zoomposition.
        /// </value>
        public double ZoomPosition
        {
            get
            {
                return (double)GetValue(ZoomPositionProperty);
            }

            set
            {
                SetValue(ZoomPositionProperty, value);
            }
        }

        /// <summary>
        /// Idenfities ZoomFactor dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ChartAxis), new PropertyMetadata(1d, new PropertyChangedCallback(OnZoomFactorChanged)));

        /// <summary>
        /// Gets or sets the ZoomFactor. This is dependency property.
        /// </summary>
        /// <remarks>
        /// Use to intialize the Zooming factor. Based on this value the chart axis is zoom.
        ///  It must be 0 to 1.
        /// </remarks>
        /// <value>
        /// The Zoom Factor.
        /// </value>
        public double ZoomFactor
        {
            get
            {
                return (double)GetValue(ZoomFactorProperty);
            }

            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value <= 0)
                {
                    value = 0.001;
                }

                SetValue(ZoomFactorProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelCornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelCornerRadiusProperty =
            DependencyProperty.Register("LabelCornerRadius", typeof(CornerRadius), typeof(ChartAxis), new PropertyMetadata(new CornerRadius()));

        /// <summary>
        /// Gets or sets the LabelCornerRadius. This is dependency property.
        /// </summary>
        /// <value>
        /// The CornerRadius.
        /// </value>
        public CornerRadius LabelCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(LabelCornerRadiusProperty);
            }

            set
            {
                SetValue(LabelCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelBorderThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBorderThicknessProperty =
            DependencyProperty.Register("LabelBorderThickness", typeof(Thickness), typeof(ChartAxis), new PropertyMetadata(new Thickness()));

        /// <summary>
        /// Gets or sets the LabelBorderThickness. This is dependency property.
        /// </summary>
        /// <value>The Thickness.</value>
        public Thickness LabelBorderThickness
        {
            get
            {
                return (Thickness)GetValue(LabelBorderThicknessProperty);
            }

            set
            {
                SetValue(LabelBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBorderBrushProperty =
            DependencyProperty.Register("LabelBorderBrush", typeof(Brush), typeof(ChartAxis), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the LabelBorderBrush. This is dependency property.
        /// </summary>
        /// <value>The Brush.</value>
        public Brush LabelBorderBrush
        {
            get
            {
                return (Brush)GetValue(LabelBorderBrushProperty);
            }

            set
            {
                SetValue(LabelBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(ChartAxis), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or sets the LabelBackground. This is dependency property.
        /// </summary>
        /// <value>The Brush.</value>
        public Brush LabelBackground
        {
            get
            {
                return (Brush)GetValue(LabelBackgroundProperty);
            }

            set
            {
                SetValue(LabelBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(ChartAxis), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the LabelForeground. This is dependency property.
        /// </summary>
        /// <value>The Brush.</value>
        public Brush LabelForeground
        {
            get
            {
                return (Brush)GetValue(LabelForegroundProperty);
            }

            set
            {
                SetValue(LabelForegroundProperty, value);
            }
        }
        
        /// <summary>
        /// Idenfities LabelFontFamily dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontFamilyProperty =
            DependencyProperty.Register("LabelFontFamily", typeof(FontFamily), typeof(ChartAxis), new PropertyMetadata(new FontFamily("Arial")));

        /// <summary>
        /// Gets or sets the LabelFontFamily. This is dependency property.
        /// </summary>
        /// <value>The FontFamily.</value>
        public FontFamily LabelFontFamily
        {
            get
            {
                return (FontFamily)GetValue(LabelFontFamilyProperty);
            }

            set
            {
                SetValue(LabelFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelFormatProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty =
            DependencyProperty.Register("LabelFormat", typeof(string), typeof(ChartAxis), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnFontInfoChanged)));

        /// <summary>
        /// Gets or sets the LabelFormat. This is dependency property.
        /// </summary>
        /// <remarks>
        /// This property helps to format the double values in chart axis.  This property
        /// can support all double formatting strings.
        /// </remarks>
        /// <value>
        /// The string.
        /// </value>
        public string LabelFormat
        {
            get
            {
                return (string)GetValue(LabelFormatProperty);
            }

            set
            {
                SetValue(LabelFormatProperty, value);
            }
        }

        /// <summary>
        /// Idenfities DateTimeRange dependency property.
        /// </summary>
        public static readonly DependencyProperty DateTimeRangeProperty =
            DependencyProperty.Register("DateTimeRange", typeof(DateTimeRange), typeof(ChartAxis), new PropertyMetadata(new DateTimeRange(), new PropertyChangedCallback(OnDateTimeRangeChanged)));

        /// <summary>
        /// Gets or sets the DateTimeRange. This is dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to give the date-time range for chart axis.
        /// </remarks>
        /// <value>
        /// The DateTimeRange.
        /// </value>
        public DateTimeRange DateTimeRange
        {
            get
            {
                return (DateTimeRange)GetValue(DateTimeRangeProperty);
            }

            set
            {
                SetValue(DateTimeRangeProperty, value);
            }
        }

        /// <summary>
        /// Idenfities DateTimeInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty DateTimeIntervalProperty =
            DependencyProperty.Register("DateTimeInterval", typeof(TimeSpan), typeof(ChartAxis), new PropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(OnDateTimeIntervalChanged)));

        /// <summary>
        /// Gets or sets the DateTimeInterval. This is dependency property.
        /// </summary>
        /// <value>The DateTimeInterval.</value>
        public TimeSpan DateTimeInterval
        {
            get
            {
                return (TimeSpan)GetValue(DateTimeIntervalProperty);
            }

            set
            {
                SetValue(DateTimeIntervalProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelDateTimeFormatProperty dependency property.
        /// </summary>
        /// <remarks>
        /// This property is helps to format the date-time values in chart axis.  This
        /// property can support all date-time formatting strings.
        /// </remarks>
        /// <returns>
        /// The String Value
        /// </returns>
        public static readonly DependencyProperty LabelDateTimeFormatProperty =
            DependencyProperty.Register("LabelDateTimeFormat", typeof(string), typeof(ChartAxis), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnFontInfoChanged)));

        /// <summary>
        /// Gets or sets the LabelDateTimeFormat. This is dependency property.
        /// </summary>
        /// <value>The string.</value>
        public string LabelDateTimeFormat
        {
            get
            {
                return (string)GetValue(LabelDateTimeFormatProperty);
            }

            set
            {
                SetValue(LabelDateTimeFormatProperty, value);
            }
        }

        /// <summary>
        /// Idenfities PrefixProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty PrefixProperty =
            DependencyProperty.Register("Prefix", typeof(string), typeof(ChartAxis), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnFontInfoChanged)));

        /// <summary>
        /// Gets or sets the Prefix. This is dependency property.
        /// </summary>
        /// <remarks>
        /// The specified contenet is added before the chart axis label values.
        /// </remarks>
        /// <value>
        /// The string.
        /// </value>
        public string Prefix
        {
            get
            {
                return (string)GetValue(PrefixProperty);
            }

            set
            {
                SetValue(PrefixProperty, value);
            }
        }
        
        /// <summary>
        /// Idenfities SuffixProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty SuffixProperty =
            DependencyProperty.Register("Suffix", typeof(string), typeof(ChartAxis), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnFontInfoChanged)));

        /// <summary>
        /// Gets or sets the Suffix. This is dependency property.
        /// </summary>
        /// <remarks>
        /// The specfied string is added after the chart axis label contents
        /// </remarks>
        /// <value>
        /// The string.
        /// </value>
        public string Suffix
        {
            get
            {
                return (string)GetValue(SuffixProperty);
            }

            set
            {
                SetValue(SuffixProperty, value);
            }
        }

        /// <summary>
        /// Idenfities ValueType dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty =
            DependencyProperty.Register("ValueType", typeof(ChartValueType), typeof(ChartAxis), new PropertyMetadata(ChartValueType.Double, new PropertyChangedCallback(OnFontInfoChanged)));

        /// <summary>
        /// Gets or sets the ValueType. This is dependency property.
        /// </summary>
        /// <remarks>
        /// Use to set the Chart axis value type.  By default axis value type is double
        /// </remarks>
        /// <value>
        /// The ChartValueType.
        /// </value>
        public ChartValueType ValueType
        {
            get
            {
                return (ChartValueType)GetValue(ValueTypeProperty);
            }

            set
            {
                SetValue(ValueTypeProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelFontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontSizeProperty =
            DependencyProperty.Register("LabelFontSize", typeof(double), typeof(ChartAxis), new PropertyMetadata(13d));

        /// <summary>
        /// Gets or sets the LabelFontSize. This is dependency property.
        /// </summary>
        /// <value>The double.</value>
        public double LabelFontSize
        {
            get
            {
                return (double)GetValue(LabelFontSizeProperty);
            }

            set
            {
                SetValue(LabelFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelFontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontWeightProperty =
            DependencyProperty.Register("LabelFontWeight", typeof(FontWeight), typeof(ChartAxis), new PropertyMetadata(new FontWeight()));

        /// <summary>
        /// Gets or sets the LabelFontWeight. This is dependency property.
        /// </summary>
        /// <value>The FontWeight.</value>
        public FontWeight LabelFontWeight
        {
            get
            {
                return (FontWeight)GetValue(LabelFontWeightProperty);
            }

            set
            {
                SetValue(LabelFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Idenfities LabelFontStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontStyleProperty =
            DependencyProperty.Register("LabelFontStyle", typeof(FontStyle), typeof(ChartAxis), new PropertyMetadata(new FontStyle()));

        /// <summary>
        /// Gets or sets the LabelFontStyle. This is dependency property.
        /// </summary>
        /// <value>The FontStyle.</value>
        public FontStyle LabelFontStyle
        {
            get
            {
                return (FontStyle)GetValue(LabelFontStyleProperty);
            }

            set
            {
                SetValue(LabelFontStyleProperty, value);
            }
        }

        /// <summary>
        /// Idenfities Area dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.Register("Area", typeof(ChartArea), typeof(ChartAxis), new PropertyMetadata(null));

        internal ItemsControl Axescontainer;
        internal ChartValueType ActualValueType = ChartValueType.Double;
        internal Thickness Axesthickness = new Thickness();
        internal ObservableCollection<ContentPresenter> axisvaluecontainer = new ObservableCollection<ContentPresenter>();

        /// <summary>
        /// Idenfities ContentPath dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentPathProperty =
    DependencyProperty.Register("ContentPath", typeof(string), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnBindingdataChanged)));

        /// <summary>
        /// Idenfities DesiredIntervalsCount dependency property.
        /// </summary>
        public static readonly DependencyProperty DesiredIntervalsCountProperty =
            DependencyProperty.Register("DesiredIntervalsCount", typeof(int), typeof(ChartAxis), new PropertyMetadata(6, new PropertyChangedCallback(OnDesiredIntervalsCountChanged)));
        internal double HeaderMaxSize=0;

        /// <summary>
        /// Idenfities m_visibleInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleIntervalProperty =
            DependencyProperty.Register("VisibleInterval", typeof(double), typeof(ChartAxis), new PropertyMetadata(1d, new PropertyChangedCallback(OnIntervalChanged)));
        /// <summary>
        ///  Identifies the Interval dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(ChartAxis), new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnIntervalValueChanged)));
        /// <summary>
        /// Idenfities IsAutoSetRange dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAutoSetRangeProperty =
            DependencyProperty.Register("IsAutoSetRange", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true, new PropertyChangedCallback(OnAutoSetRangeChanged)));
        internal bool Isindexedseries = false;
        internal bool _IsSortedSeries = false;
        internal Direction _SortingDirection = Direction.Ascending;
        internal SortingAxis _SortingAxis = SortingAxis.X;
        /// <summary>
        /// Idenfities LabelsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelsSourceProperty =
   DependencyProperty.Register("LabelsSource", typeof(IEnumerable), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelSourceChanged)));

        /// <summary>
        /// Idenfities LineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeProperty =
DependencyProperty.Register("LineStroke", typeof(Brush), typeof(ChartAxis), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Idenfities LineStrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeThicknessProperty =
DependencyProperty.Register("LineStrokeThickness", typeof(double), typeof(ChartAxis), new PropertyMetadata(1d));

        /// <summary>
        /// Idenfities GridLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLineStrokeProperty =
DependencyProperty.Register("GridLineStroke", typeof(Brush), typeof(ChartAxis), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLineStyleChanged)));

        /// <summary>
        /// Idenfities OriginLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty OriginLineStrokeProperty =
DependencyProperty.Register("OriginLineStroke", typeof(Brush), typeof(ChartAxis), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLineStyleChanged)));
        /// <summary>
        ///  Identifies the MinorGridLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorGridLineStrokeProperty =
DependencyProperty.Register("MinorGridLineStroke", typeof(Brush), typeof(ChartAxis), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLineStyleChanged)));
        /// <summary>
        /// Identifies the MinorGridLineStrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorGridLineStrokeThicknessProperty =
DependencyProperty.Register("MinorGridLineStrokeThickness", typeof(double), typeof(ChartAxis), new PropertyMetadata(0.25, new PropertyChangedCallback(OnLineStyleChanged)));

        /// <summary>
        /// Idenfities GridLineStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLineStyleProperty =
DependencyProperty.Register("GridLineStyle", typeof(DoubleCollection), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnLineStyleTypeChanged)));

        /// <summary>
        /// Idenfities GridLineStrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLineStrokeThicknessProperty =
DependencyProperty.Register("GridLineStrokeThickness", typeof(double), typeof(ChartAxis), new PropertyMetadata(1d, new PropertyChangedCallback(OnLineStyleChanged)));

        /// <summary>
        /// Idenfities OriginLineStrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty OriginLineStrokeThicknessProperty =
DependencyProperty.Register("OriginLineStrokeThickness", typeof(double), typeof(ChartAxis), new PropertyMetadata(1d, new PropertyChangedCallback(OnLineStyleChanged)));
        internal List<object> AxisDataContents
        {
            get;
            set;
        }

        internal ChartPointsCollection SeriesData
        {
            get;
            set;
        }

        internal List<double> AxisDataPosition
        {
            get;
            set;
        }

        internal List<object> IndexDataContents
        {
            get;
            set;
        }

        internal double MaxSize=0;

        /// <summary>
        /// Idenfities OpposedPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty OpposedPositionProperty =
            DependencyProperty.Register("OpposedPosition", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnOpposedPositionChanged)));

        /// <summary>
        /// Idenfities Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartAxis), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Idenfities PositionPath dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionPathProperty =
DependencyProperty.Register("PositionPath", typeof(string), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnBindingdataChanged)));

        /// <summary>
        /// Idenfities Range dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(new DoubleRange(0, 1), new PropertyChangedCallback(OnRangeChanged)));

        ////        internal double MinValue;
        ////        internal double Maxvalue;
        internal double SegmentMaxValue=0;
        internal double SegmentMinValue=0;

        /// <summary>
        /// Idenfities ShowGridLines dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowGridLinesProperty =
                    DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true, new PropertyChangedCallback(OnLineStyleChanged)));

        /// <summary>
        /// Idenfities ShowOriginLine dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowOriginLineProperty =
                    DependencyProperty.Register("ShowOriginLine", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnLineStyleChanged)));

        /// <summary>
        /// Idenfities SmallTicksStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallTicksStrokeProperty =
DependencyProperty.Register("SmallTicksStroke", typeof(Brush), typeof(ChartAxis), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Idenfities SmallTicksStrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallTicksStrokeThicknessProperty =
DependencyProperty.Register("SmallTicksStrokeThickness", typeof(double), typeof(ChartAxis), new PropertyMetadata(1d, new PropertyChangedCallback(OnLineStyleChanged)));
        /// <summary>
        ///  Identifies the TickLineStrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty TickLineStrokeThicknessProperty =
DependencyProperty.Register("TickLineStrokeThickness", typeof(double), typeof(ChartAxis), new PropertyMetadata(1d, new PropertyChangedCallback(OnLineStyleChanged)));
        internal double userinterval = double.NaN;

        /// <summary>
        /// Idenfities VisibleRange dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleRangeProperty =
                DependencyProperty.Register("VisibleRange", typeof(DoubleRange), typeof(ChartAxis), new PropertyMetadata(DoubleRange.Empty, new PropertyChangedCallback(OnRangeChanged)));
        /// <summary>
        /// Idenfities SmallTicksRequired dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallTicksRequiredProperty =
                   DependencyProperty.Register("SmallTicksRequired", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnLineStyleChanged)));
        /// <summary>
        /// Idenfities SmallTicksPerInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallTicksPerIntervalProperty =
DependencyProperty.Register("SmallTicksPerInterval", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d, new PropertyChangedCallback(OnLabelsChanged)));
        /// <summary>
        /// Identifies the ChartLabelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartLabelPositionProperty =
          DependencyProperty.Register("ChartLabelPosition", typeof(LabelPositions), typeof(ChartAxis), new PropertyMetadata(LabelPositions.Outside, new PropertyChangedCallback(OnLabelsChanged)));

        /// <summary>
        /// Get or Set ChartlabelPosition property
        /// </summary>
        public LabelPositions ChartLabelPosition
        {
            get { return (LabelPositions)GetValue(ChartLabelPositionProperty); }
            set { SetValue(ChartLabelPositionProperty, value); }
        }
        /// <summary>
        /// Identifies the ChartTickLinesPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartTickLinesPositionProperty =
          DependencyProperty.Register("ChartTickLinesPosition", typeof(AxisPositions), typeof(ChartAxis), new PropertyMetadata(AxisPositions.Outside, new PropertyChangedCallback(OnLabelsChanged)));
        /// <summary>
        /// Get or Set ChartTickLinesposition
        /// </summary>
        public AxisPositions ChartTickLinesPosition
        {
            get { return (AxisPositions)GetValue(ChartTickLinesPositionProperty); }
            set { SetValue(ChartTickLinesPositionProperty, value); }
        }
        /// <summary>
        /// Identifies the AxisLabels dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisLabelsProperty =
         DependencyProperty.Register("AxisLabels", typeof(AxisLabels), typeof(ChartAxis), new PropertyMetadata(AxisLabels.Low, new PropertyChangedCallback(OnLabelsChanged)));
        /// <summary>
        /// Get or Set AxisLabelsProperty
        /// </summary>
        public AxisLabels AxisLabels
        {
            get { return (AxisLabels)GetValue(AxisLabelsProperty); }
            set { SetValue(AxisLabelsProperty, value); }
        }
        /// <summary>
        /// Identifies the AxisVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisVisibilityProperty =
        DependencyProperty.Register("AxisVisibility", typeof(Visibility), typeof(ChartAxis), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnLabelsChanged)));
        /// <summary>
        /// Get or Set AxisVisibility
        /// </summary>
        public Visibility AxisVisibility
        {
            get { return (Visibility)GetValue(AxisVisibilityProperty); }
            set { SetValue(AxisVisibilityProperty, value); }
        }
        /// <summary>
        ///  Identifies the HeaderPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderPositionProperty =
         DependencyProperty.Register("HeaderPosition", typeof(AxisPositions), typeof(ChartAxis), new PropertyMetadata(AxisPositions.Outside, new PropertyChangedCallback(OnLabelsChanged)));
        /// <summary>
        /// Get or Set HeaderPositionProperty
        /// </summary>
        public AxisPositions HeaderPosition
        {
            get { return (AxisPositions)GetValue(HeaderPositionProperty); }
            set { SetValue(HeaderPositionProperty, value); }
        }
        /// <summary>
        /// Identifies the ChatTickLinesRange dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartTickLinesRangeProperty =
          DependencyProperty.Register("ChartTickLinesRange", typeof(double), typeof(ChartAxis), new PropertyMetadata(0.5d, new PropertyChangedCallback(OnLabelsChanged)));

        /// <summary>
        /// Get or Set ChartTickLinesRangeProperty
        /// </summary>
        public double ChartTickLinesRange
        {
            get { return (double)GetValue(ChartTickLinesRangeProperty); }
            set { SetValue(ChartTickLinesRangeProperty, value); }
        }
        /// <summary>
        ///  Identifies the TickSize dependency property.
        /// </summary>
        public static readonly DependencyProperty TickSizeProperty =
         DependencyProperty.Register("TickSize", typeof(double), typeof(ChartAxis), new PropertyMetadata(5d, new PropertyChangedCallback(OnLineStyleChanged)));

        /// <summary>
        /// Get or Set TickSizeProperty
        /// </summary>
        public double TickSize
        {
            get { return (double)GetValue(TickSizeProperty); }
            set { SetValue(TickSizeProperty, value); }
        }

        internal static readonly DependencyProperty LabelTransformXProperty =
         DependencyProperty.Register("LabelTransformX", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        internal double LabelTransformX
        {
            get { return (double)GetValue(LabelTransformXProperty); }
            set { SetValue(LabelTransformXProperty, value); }
        }

        internal static readonly DependencyProperty LabelTransformYProperty =
         DependencyProperty.Register("LabelTransformY", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        internal double LabelTransformY
        {
            get { return (double)GetValue(LabelTransformYProperty); }
            set { SetValue(LabelTransformYProperty, value); }
        }
        internal static readonly DependencyProperty TickTransformXProperty =
         DependencyProperty.Register("TickTransformX", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        internal double TickTransformX
        {
            get { return (double)GetValue(TickTransformXProperty); }
            set { SetValue(TickTransformXProperty, value); }
        }

        internal static readonly DependencyProperty TickTransformYProperty =
         DependencyProperty.Register("TickTransformY", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        internal double TickTransformY
        {
            get { return (double)GetValue(TickTransformYProperty); }
            set { SetValue(TickTransformYProperty, value); }
        }

        internal static readonly DependencyProperty HeaderTransformXProperty =
         DependencyProperty.Register("HeaderTransformX", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        internal double HeaderTransformX
        {
            get { return (double)GetValue(HeaderTransformXProperty); }
            set { SetValue(HeaderTransformXProperty, value); }
        }

        internal static readonly DependencyProperty HeaderTransformYProperty =
         DependencyProperty.Register("HeaderTransformY", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));

        internal double HeaderTransformY
        {
            get { return (double)GetValue(HeaderTransformYProperty); }
            set { SetValue(HeaderTransformYProperty, value); }
        }
        /// <summary>
        ///  Identifies the SmallTickSize dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallTickSizeProperty =
         DependencyProperty.Register("SmallTickSize", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d, new PropertyChangedCallback(OnLineStyleChanged)));
        /// <summary>
        /// Get or Set SmallTickSizeProperty
        /// </summary>
        public double SmallTickSize
        {
            get { return (double)GetValue(SmallTickSizeProperty); }
            set { SetValue(SmallTickSizeProperty, value); }
        }
        /// <summary>
        ///  Identifies the TickLinesStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty TickLineStrokeProperty =
         DependencyProperty.Register("TickLineStroke", typeof(Brush), typeof(ChartAxis), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLineStyleChanged)));
        /// <summary>
        /// Get or Set TickLineStrokeProperty
        /// </summary>
        public Brush TickLineStroke
        {
            get { return (Brush)GetValue(TickLineStrokeProperty); }
            set { SetValue(TickLineStrokeProperty, value); }
        }

        /// <summary>
        /// Get or Set ShowEdgeLabelsProperty
        /// </summary>
        public bool ShowEdgeLabels
        {
            get { return (bool)GetValue(ShowEdgeLabelsProperty); }
            set { SetValue(ShowEdgeLabelsProperty, value); }
        }

         
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowEdgeLabels.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowEdgeLabelsProperty =
            DependencyProperty.Register("ShowEdgeLabels", typeof(bool), typeof(ChartAxis), new PropertyMetadata(true));

        

        #region ScaleBreak Properties

        internal bool m_enableBreaks;
        internal double m_autoBreakThresholdCoefficient = 0.2d;
        internal double m_sumBreaks = 0d;
        internal ChartScaleBreak m_autoScaleBreak = null;
        
        /// <summary>
        /// Initializes m_ranges
        /// </summary>
        internal List<DoubleRange> m_manualBreakRanges = new List<DoubleRange>();

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableBreaks.
        /// </summary>
        public static readonly DependencyProperty EnableBreaksProperty =
            DependencyProperty.Register("EnableBreaks", typeof(bool), typeof(ChartAxis), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableBreaksChanged)));

        /// <summary>
        /// Gets or sets value for EnableBreaks
        /// </summary>
        public bool EnableBreaks
        {
            get { return (bool)GetValue(EnableBreaksProperty); }
            set { SetValue(EnableBreaksProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BreaksMode.
        /// </summary>
        public static readonly DependencyProperty BreaksModeProperty =
            DependencyProperty.Register("BreaksMode", typeof(ScaleBreaksModes), typeof(ChartAxis), new PropertyMetadata(ScaleBreaksModes.Manual, new PropertyChangedCallback(OnBreaksModeChanged)));

        /// <summary>
        /// Gets or sets value for BreaksMode
        /// </summary>
        public ScaleBreaksModes BreaksMode
        {
            get { return (ScaleBreaksModes)GetValue(BreaksModeProperty); }
            set { SetValue(BreaksModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ManualBreaks.
        /// </summary>
        public static readonly DependencyProperty ManualBreaksProperty =
            DependencyProperty.Register("ManualBreaks", typeof(ChartScaleBreaksCollection), typeof(ChartAxis), new PropertyMetadata(new ChartScaleBreaksCollection(), new PropertyChangedCallback(OnManualBreaksChanged)));

        /// <summary>
        /// Gets or sets value for BreaksMode
        /// </summary>
        public ChartScaleBreaksCollection ManualBreaks
        {
            get { return (ChartScaleBreaksCollection)GetValue(ManualBreaksProperty); }
            set { SetValue(ManualBreaksProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoBreakThresholdCoefficient.
        /// </summary>
        public static readonly DependencyProperty AutoBreakThresholdCoefficientProperty =
            DependencyProperty.Register("AutoBreakThresholdCoefficient", typeof(double), typeof(ChartAxis), new PropertyMetadata(0.2d, new PropertyChangedCallback(OnAutoBreakThresholdCoefficientChanged)));

        /// <summary>
        /// Gets or sets value for AutoBreakThresholdCoefficient
        /// </summary>
        public double AutoBreakThresholdCoefficient
        {
            get { return (double)GetValue(AutoBreakThresholdCoefficientProperty); }
            set { SetValue(AutoBreakThresholdCoefficientProperty, value); }
        }
        internal static readonly DependencyProperty VerticalLableMarginProperty = DependencyProperty.Register("VerticalLableMargin", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));
        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        /// <value>The X value.</value>
        internal double VerticalLableMargin
        {
            get { return (double)GetValue(VerticalLableMarginProperty); }
            set { SetValue(VerticalLableMarginProperty, value); }
        }

        internal static readonly DependencyProperty HorizontalLableMarginProperty = DependencyProperty.Register("HorizontalLableMargin", typeof(double), typeof(ChartAxis), new PropertyMetadata(0d));
        /// <summary>
        /// Gets or sets the HorizontalLableMargin value.
        /// </summary>
        /// <value>The HorizontalLableMargin value.</value>
        internal double HorizontalLableMargin
        {
            get { return (double)GetValue(HorizontalLableMarginProperty); }
            set { SetValue(HorizontalLableMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalLabelContent, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty LabelContentProperty = DependencyProperty.Register("LabelContent", typeof(ChartPoint), typeof(ChartAxis), new PropertyMetadata(new ChartPoint()));
        /// <summary>
        /// Gets or sets the VerticalLabelContent value.
        /// </summary>
        /// <value>The VerticalLabelContent value.</value>
        internal ChartPoint LabelContent
        {
            get { return (ChartPoint)GetValue(LabelContentProperty); }
            set { SetValue(LabelContentProperty, value); }
        }

        /// <summary>
        /// Identifies the CursorLabelTemplate, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty CursorLabelTemplateProperty = DependencyProperty.Register("CursorLabelTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the CursorLabelTemplate value.
        /// </summary>
        /// <value>The CursorLabelTemplate value.</value>
        internal DataTemplate CursorLabelTemplate
        {
            get { return (DataTemplate)GetValue(CursorLabelTemplateProperty); }
            set { SetValue(CursorLabelTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalLabelTemplate, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty VerticalLabelTemplateProperty = DependencyProperty.Register("VerticalLabelTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelTemplateChanged)));
        /// <summary>
        /// Gets or sets the VerticalLabelTemplate value.
        /// </summary>
        /// <value>The VerticalLabelTemplate value.</value>
        public DataTemplate VerticalLabelTemplate
        {
            get { return (DataTemplate)GetValue(VerticalLabelTemplateProperty); }
            set { SetValue(VerticalLabelTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalLabelTemplate, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty HorizontalLabelTemplateProperty = DependencyProperty.Register("HorizontalLabelTemplate", typeof(DataTemplate), typeof(ChartAxis), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelTemplateChanged)));
        /// <summary>
        /// Gets or sets the HorizontalLabelTemplate value.
        /// </summary>
        /// <value>The HorizontalLabelTemplate value.</value>
        public DataTemplate HorizontalLabelTemplate
        {
            get { return (DataTemplate)GetValue(HorizontalLabelTemplateProperty); }
            set { SetValue(HorizontalLabelTemplateProperty, value); }
        }
        /// <summary>
        ///  Identifies the VerticalLabelVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalLabelVisibilityProperty =
            DependencyProperty.Register("VerticalLabelVisibility", typeof(Visibility), typeof(ChartAxis),
            new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnLabelVisibilityChanged)));
       /// <summary>
        /// Get or Set VerticalLabelVisibilityProperty
       /// </summary>
        public Visibility VerticalLabelVisibility
        {
            get { return (Visibility)GetValue(VerticalLabelVisibilityProperty); }
            set { SetValue(VerticalLabelVisibilityProperty, value); }
        }
        /// <summary>
        ///  Identifies the HorizontalLabelVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalLabelVisibilityProperty =
            DependencyProperty.Register("HorizontalLabelVisibility", typeof(Visibility), typeof(ChartAxis),
            new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnLabelVisibilityChanged)));
        /// <summary>
        /// Get or Set HorizontalLabelVisibilityProperty
        /// </summary>
        public Visibility HorizontalLabelVisibility
        {
            get { return (Visibility)GetValue(HorizontalLabelVisibilityProperty); }
            set { SetValue(HorizontalLabelVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifies the CursorLabelVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty CursorLabelVisibilityProperty =
            DependencyProperty.Register("CursorLabelVisibility", typeof(Visibility), typeof(ChartAxis),
            new PropertyMetadata(Visibility.Collapsed));
        /// <summary>
        /// Get or Set CursorLabelVisibilityProperty
        /// </summary>
        public Visibility CursorLabelVisibility
        {
            get { return (Visibility)GetValue(CursorLabelVisibilityProperty); }
            set { SetValue(CursorLabelVisibilityProperty, value); }
        }

        #endregion

        internal DoubleRange m_VisibleRange = DoubleRange.Empty;
        /// <summary>
        /// Called when instance created for ChartAxis
        /// </summary>
        public ChartAxis()
        {
            DefaultStyleKey = typeof(ChartAxis);
            this.AxisDataContents = new List<object>();
            this.AxisDataPosition = new List<double>();
            this.IndexDataContents = new List<object>();
            this.ZoomRange = DoubleRange.Empty;
            this.ZoomVisibleRange = DoubleRange.Empty;
            this.PreviousRange = DoubleRange.Empty;
            this.StripLines.CollectionChanged += new NotifyCollectionChangedEventHandler(StripLines_CollectionChanged);
            this.Loaded += new RoutedEventHandler(this.Axis_Loaded);
            this.IsRefreshItems = true;

            this.AxisLabelMaxSize = 0d;
        }

        void CustomLabels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Area != null)
            {
                this.Area.LoadArea();
            }
        }

        private void StripLines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ChartStripLine stripline in e.NewItems)
                {
                    if (stripline.Axis == null)
                    {
                        stripline.Axis = this;
                        //if (this.Area != null)
                          //  this.Area.LoadArea();
                    }
                }
                if (this.Area != null && this.Area.stripLinePanel!=null)
                {
                    //this.Area.LoadArea();
                    this.Area.stripLinePanel.InvalidateMeasure();
                }
            }
            if (e.NewItems == null)
            {
                if (this.Area != null && this.Area.stripLinePanel != null)
                {
                    //this.Area.LoadArea();
                    this.Area.stripLinePanel.InvalidateMeasure();
                }

            }
        }

        /// <summary>
        /// Executes when EnableBreaks changed 
        /// </summary>
        private static void OnEnableBreaksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue == true)
                {
                    axis.ManualBreaks.CollectionChanged += new NotifyCollectionChangedEventHandler(axis.ManualBreaks_CollectionChanged);
                }                
            }
            if (axis != null)
            {
                axis.m_enableBreaks = (axis.ZoomFactor != 1d) ? false : (bool)e.NewValue;
                axis.UpdateScaleBreaks();
            }
        }

        private static void OnAutoBreakThresholdCoefficientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.m_autoBreakThresholdCoefficient = ChartMath.MinMax((double)e.NewValue, 0, 1);
                axis.m_enableBreaks = (axis.m_autoBreakThresholdCoefficient == 0) ? false : axis.EnableBreaks;
                axis.UpdateScaleBreaks();
            }
        }

        /// <summary>
        /// Executes when BreaksMode Changed
        /// </summary>
        private static void OnBreaksModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.UpdateScaleBreaks();
            }
        }

        /// <summary>
        /// Executes when ManaualBreaks changed
        /// </summary>
        private static void OnManualBreaksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.UpdateScaleBreaks();
            }
        }

        /// <summary>
        /// Executes when ManualBreak changed
        /// </summary>
        private void ManualBreaks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                if (this.Area != null)
                {
                    this.Area.Breaks.Clear();
                    for(int i=0; i<e.NewItems.Count; i++)
                    {
                        this.Area.Breaks.Add((ChartScaleBreak)e.NewItems[i]);
                    }
                }
                UpdateScaleBreaks();
            }
        }

        internal void UpdateScaleBreaks()
        {
            if (this.Area != null)
            {
                this.RefreshAxis();
                if (this.tickLinesPanel != null)
                    this.tickLinesPanel.InvalidateMeasure();

                this.m_manualBreakRanges.Clear();
                this.Area.Breaks.Clear();
                if (this.BreaksMode == ScaleBreaksModes.Manual)
                {
                    foreach (ChartScaleBreak scaleBreak in this.ManualBreaks)
                    {
                        scaleBreak.m_axis = this;
                        this.m_manualBreakRanges.Add(scaleBreak.m_breakRange);
                        this.Area.Breaks.Add(scaleBreak);
                        if(scaleBreak.scaleBreakPanel != null)
                            scaleBreak.scaleBreakPanel.InvalidateMeasure();
                    }
                    this.CalculateSumBreaks();

                }

                else if (this.BreaksMode == ScaleBreaksModes.Auto && this.m_autoBreakThresholdCoefficient > 0 && this.Area.Series != null)
                {
                    ChartScaleBreak scaleBrk = new ChartScaleBreak() { m_axis = this };
                    scaleBrk.Compute(this.Area.Series);
                    this.m_autoScaleBreak = scaleBrk;

                    for (int i = 0, ci = scaleBrk.m_segments.Count - 1; i < ci; i++)
                    {
                        ChartScaleBreak scaleBreak = new ChartScaleBreak() { m_axis = this, m_segments = scaleBrk.m_segments };

                        ChartScaleBreakSegment segment1 = scaleBrk.m_segments[i] as ChartScaleBreakSegment;
                        ChartScaleBreakSegment segment2 = scaleBrk.m_segments[i + 1] as ChartScaleBreakSegment;

                        scaleBreak.m_breakRange = new DoubleRange(segment1.Range.End, segment2.Range.Start);

                        this.Area.Breaks.Add(scaleBreak);
                        if (scaleBreak.scaleBreakPanel != null)
                            scaleBreak.scaleBreakPanel.InvalidateMeasure();
                    }
                }

                if (this.Area.Series != null)
                {
                    foreach (ChartSeries series in this.Area.Series)
                    {
                        series.Area.LoadOnlySeries();
                    }
                }

            }
        }

        /// <summary>
        /// Computes the sum of breaks.
        /// </summary>
        internal void CalculateSumBreaks()
        {
            this.m_sumBreaks = 0;
            foreach(DoubleRange brkRange in this.m_manualBreakRanges)
            {
                this.m_sumBreaks += brkRange.Delta;
            }
            
        }

        internal double ValueToCoefficient(double value)
        {
            double result = 0d;
            if (this.m_enableBreaks)
            {
                switch (this.BreaksMode)
                {
                    case ScaleBreaksModes.None:
                        result = (value - VisibleRange.Start) / VisibleRange.Delta;
                        break;

                    case ScaleBreaksModes.Auto:
                        if (m_autoScaleBreak != null)
                            result = m_autoScaleBreak.AutoValueToCoefficient(value);
                        else if (this.Area != null && this.Area.Series != null)
                        {
                            ChartScaleBreak scaleBreak = new ChartScaleBreak() { m_axis = this };
                            scaleBreak.Compute(this.Area.Series);
                            result = scaleBreak.AutoValueToCoefficient(value);
                        }
                        break;

                    case ScaleBreaksModes.Manual:
                        {
                            double val = value - VisibleRange.Start;
                            foreach (DoubleRange range in this.m_manualBreakRanges)
                            {
                                if (range.Start < (value - VisibleRange.Start))
                                {
                                    val -= Math.Min(range.Delta, (value - VisibleRange.Start) - range.Start);
                                }
                            }
                            result = val / (VisibleRange.Delta - this.m_sumBreaks);
                        }
                        break;
                }
            }
            return this.IsInversed ? 1d - result : result;
        }

        internal double CoefficientToValue(double coefficient)
        {
            double value = 0d;
            double result = this.IsInversed ? 1d - coefficient : coefficient;
            if (this.m_enableBreaks)
            {
                switch (this.BreaksMode)
                {
                    case ScaleBreaksModes.None:
                        result = VisibleRange.Start + VisibleRange.Delta * value;
                        break;

                    case ScaleBreaksModes.Auto:
                        if (m_autoScaleBreak != null)
                            result = m_autoScaleBreak.AutoCoefficientToValue(value);
                        else if (this.Area != null && this.Area.Series != null)
                        {
                            ChartScaleBreak scaleBreak = new ChartScaleBreak() { m_axis = this };
                            scaleBreak.Compute(this.Area.Series);
                            result = scaleBreak.AutoCoefficientToValue(value);
                        }
                        break;

                    case ScaleBreaksModes.Manual:
                        {
                            double val = value - VisibleRange.Start;
                            foreach (DoubleRange range in this.m_manualBreakRanges)
                            {
                                if (range.Start < (value - VisibleRange.Start))
                                {
                                    val -= Math.Min(range.Delta, (value - VisibleRange.Start) - range.Start);
                                }
                            }
                            result = val / (VisibleRange.Delta - this.m_sumBreaks);
                        }
                        break;
                }
            }
            return result;
        }

        /// <summary>et
        /// Generate AutoSetRange  dependency property Changed.
        /// </summary>
        public event PropertyChangedCallback AutoSetRangeChanged;

        /// <summary>
        /// Generate DesiredIntervalsCount dependency property Changed.
        /// </summary>
        public event PropertyChangedCallback DesiredIntervalsCountChanged;

        /// <summary>
        /// Generate m_visibleInterval dependency property Changed.
        /// </summary>
        public event PropertyChangedCallback IntervalChanged;

        /// <summary>
        /// Generate LineStyle dependency property Changed.
        /// </summary>
        public event PropertyChangedCallback LineStyleChanged;

        /// <summary>
        /// Generate OpposedPosition dependency property Changed.
        /// </summary>
        public event PropertyChangedCallback OpposedPositionChanged;

        /// <summary>
        /// Generate Range dependency property Changed.
        /// </summary>
        public event PropertyChangedCallback RangeChanged;

        /// <summary>
        /// Generate Range dependency property Changed.
        /// </summary>
        public event PropertyChangedCallback SelectedRangeChanged;

        /// <summary>
        /// Gets or sets the Area. This is property.
        /// </summary>
        /// <value>The chart Area.</value>
        public ChartArea Area
        {
            get
            {
                if (m_area == null)
                {
                    m_area = this.GetParentArea();
                }

                return m_area;
            }

            set
            {
                m_area = value;
            }
        }

        /// <summary>
        /// Gets or sets the ContenetPath. This is dependency property.
        /// </summary>
        /// <remarks>
        /// When LabelSource property is set then ContentPath is used to determine the
        /// content for axis labels.
        /// </remarks>
        /// <value>
        /// The string.
        /// </value>
        public string ContentPath
        {
            get
            {
                return (string)GetValue(ContentPathProperty);
            }

            set
            {
                SetValue(ContentPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DesiredIntervalsCount. This is dependency property.
        /// </summary>
        /// <remarks>
        /// Property indicates quantity of intervals that axis range should be divided by. 
        /// The IsAutoSetRange is false then this property value is not effective.
        /// </remarks>
        /// <value>
        /// The DesiredIntervalsCount.
        /// </value>
        public int DesiredIntervalsCount
        {
            get
            {
                return (int)GetValue(DesiredIntervalsCountProperty);
            }

            set
            {
                if (value > 0)
                {
                    SetValue(DesiredIntervalsCountProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the m_visibleInterval. This is dependency property.
        /// </summary>
        /// <remarks>
        /// This is increment value from the Range property Start value to End value.
        /// </remarks>
        /// <value>
        /// The m_visibleInterval value.
        /// </value>
        public double VisibleInterval
        {
            get
            {
                return (double)GetValue(VisibleIntervalProperty);
            }

            set
            {
                if (this.LabelsSource == null)
                {
                    if (value < 0)
                    {
                        value = value * -1;
                    }
                }

                if (value != 0 && value != this.VisibleInterval)
                {
                    SetValue(VisibleIntervalProperty, value);
                }
            }
        }
        /// <summary>
        /// Get or Set IntervalProperty
        /// </summary>
        public double Interval
        {
            get
            {
                return (double)GetValue(IntervalProperty);
            }

            set
            {
                SetValue(IntervalProperty, value);
            }
        }

        /// <summary>
        /// Get or Set SmallTicksRequiredProperty
        /// </summary>
        public bool SmallTicksRequired
        {
            get
            {
                return (bool)GetValue(SmallTicksRequiredProperty);
            }

            set
            {
                SetValue(SmallTicksRequiredProperty, value);
            }
        }
        /// <summary>
        /// Get or Set SmallTicksPerIntervalProperty
        /// </summary>
        public double SmallTicksPerInterval
        {
            get
            {
                return (double)GetValue(SmallTicksPerIntervalProperty);
            }

            set
            {
                SetValue(SmallTicksPerIntervalProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the IsAutoSetRange. This is dependency
        /// property.
        /// </summary>
        /// <remarks>
        /// The IsAutoSetRange is true then Range property value is automatically determine
        /// by using the data provided in series.  Otherwise user set Range value is render
        /// in output.
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool IsAutoSetRange
        {
            get
            {
                return (bool)GetValue(IsAutoSetRangeProperty);
            }

            set
            {
                SetValue(IsAutoSetRangeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LabelsSource. This is dependency property.
        /// </summary>
        /// <remarks>
        /// The LabelSource property is used to set the collection object is bind to the
        /// chart axis.  It can support any IEnumerable type.
        /// </remarks>
        /// <value>
        /// The LabelsSource value.
        /// </value>
        public IEnumerable LabelsSource
        {
            get
            {
                return (IEnumerable)GetValue(LabelsSourceProperty);
            }

            set
            {
                SetValue(LabelsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the GridLineStroke. This is dependency property.
        /// </summary>
        /// <value>The GridLineStroke value is Brush color.</value>
        public Brush GridLineStroke
        {
            get
            {
                return (Brush)GetValue(GridLineStrokeProperty);
            }

            set
            {
                SetValue(GridLineStrokeProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the OriginLineStroke. This is dependency property.
        /// </summary>
        /// <value>The OriginLineStroke value is Brush color.</value>
        public Brush OriginLineStroke
        {
            get
            {
                return (Brush)GetValue(OriginLineStrokeProperty);
            }

            set
            {
                SetValue(OriginLineStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MinorGridLineStroke. This is dependency property.
        /// </summary>
        /// <value>The MinorGridLineStroke value is Brush color.</value>
        public Brush MinorGridLineStroke
        {
            get
            {
                return (Brush)GetValue(MinorGridLineStrokeProperty);
            }

            set
            {
                SetValue(MinorGridLineStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MinorGridLineThickness. This is dependency property.
        /// </summary>
        /// <value>The MinorGridLineThickness value is Brush color.</value>MinorGridLineThickness
        public double MinorGridLineStrokeThickness
        {
            get
            {
                return (double)GetValue(MinorGridLineStrokeThicknessProperty);
            }

            set
            {
                SetValue(MinorGridLineStrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the GridLineStyle. This is dependency property.
        /// </summary>
        /// <value>The GridLineStyle value.</value>
        public DoubleCollection GridLineStyle
        {
            get
            {
                return (DoubleCollection)GetValue(GridLineStyleProperty);
            }

            set
            {
                SetValue(GridLineStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the GridLineStrokeThickness. This is dependency property.
        /// </summary>
        /// <value>The GridLineStrokeThickness value.</value>
        public double GridLineStrokeThickness
        {
            get
            {
                return (double)GetValue(GridLineStrokeThicknessProperty);
            }

            set
            {
                SetValue(GridLineStrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the OriginLineStrokeThickness. This is dependency property.
        /// </summary>
        /// <value>The OriginLineStrokeThickness value.</value>
        public double OriginLineStrokeThickness
        {
            get
            {
                return (double)GetValue(OriginLineStrokeThicknessProperty);
            }

            set
            {
                SetValue(OriginLineStrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LineStroke. This is dependency property.
        /// </summary>
        /// <value>The LineStroke value is Brush color.</value>
        public Brush LineStroke
        {
            get
            {
                return (Brush)GetValue(LineStrokeProperty);
            }

            set
            {
                SetValue(LineStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the LineStrokeThickness. This is dependency property.
        /// </summary>
        /// <value>The LineStrokeThickness value.</value>
        public double LineStrokeThickness
        {
            get
            {
                return (double)GetValue(LineStrokeThicknessProperty);
            }

            set
            {
                SetValue(LineStrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the OpposedPosition. This is dependency property.
        /// </summary>
        /// <value>The OpposedPosition value is true or false.</value>
        public bool OpposedPosition
        {
            get
            {
                return (bool)GetValue(OpposedPositionProperty);
            }

            set
            {
                SetValue(OpposedPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Orientation. This is dependency property.
        /// </summary>
        /// <value>The Orientation value.</value>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the PositionPath. This is dependency property.
        /// </summary>
        /// <remarks>
        /// When LabelSource property is set, the position path is used to determine the
        /// position of labels.
        /// </remarks>
        /// <value>
        /// The PositionPath value is string.
        /// </value>
        public string PositionPath
        {
            get
            {
                return (string)GetValue(PositionPathProperty);
            }

            set
            {
                SetValue(PositionPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Range. This is dependency property.
        /// </summary>
        /// <remarks>
        /// The Range contains the Chart axis start and End values.  By default Range value
        /// is 0 to 1.
        /// </remarks>
        /// <value>
        /// The Range value.
        /// </value>
        public DoubleRange Range
        {
            get
            {
                return (DoubleRange)GetValue(RangeProperty);
            }

            set
            {
                if (value.Start != value.End)
                {
                    SetValue(RangeProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ShowGridLines. This is dependency property.
        /// </summary>
        /// <value>The ShowGridLines value is true or false.</value>
        public bool ShowGridLines
        {
            get
            {
                return (bool)GetValue(ShowGridLinesProperty);
            }

            set
            {
                SetValue(ShowGridLinesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ShowOriginLine. This is dependency property.
        /// </summary>
        /// <value>The ShowOriginLine value is true or false.</value>
        public bool ShowOriginLine
        {
            get
            {
                return (bool)GetValue(ShowOriginLineProperty);
            }

            set
            {
                SetValue(ShowOriginLineProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the SmallTicksStroke. This is dependency property.
        /// </summary>
        /// <value>The SmallTicksStroke value is brush color.</value>
        public Brush SmallTicksStroke
        {
            get
            {
                return (Brush)GetValue(SmallTicksStrokeProperty);
            }

            set
            {
                SetValue(SmallTicksStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the SmallTicksStrokeThickness. This is dependency property.
        /// </summary>
        /// <value>The SmallTicksStrokeThickness value.</value>
        public double SmallTicksStrokeThickness
        {
            get
            {
                return (double)GetValue(SmallTicksStrokeThicknessProperty);
            }

            set
            {
                SetValue(SmallTicksStrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the TickLineStrokeThickness. This is dependency property.
        /// </summary>
        /// <value>The TickLineStrokeThickness value.</value>
        public double TickLineStrokeThickness
        {
            get
            {
                return (double)GetValue(TickLineStrokeThicknessProperty);
            }

            set
            {
                SetValue(TickLineStrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the VisibleRange. This is dependency property.
        /// </summary>
        /// <value>The VisibleRange value.</value>
        public DoubleRange VisibleRange
        {
            get
            {
                return (DoubleRange)GetValue(VisibleRangeProperty);
            }

            set
            {
                SetValue(VisibleRangeProperty, value);
            }
        }

        /// <summary>
        /// Axis Load event
        /// </summary>
        private void Axis_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.axisvaluecontainer.Count != 0)
            {
                this.Axescontainer.ItemsSource = this.axisvaluecontainer;
            }

            if (this.Area != null)
            {
                ParentArea = this.Area;
                if (this.Area.AreaType != ChartAxesType.RadarAxes && this.Area.AreaType != ChartAxesType.PolarAxes)
                {
                    ////if (ParentArea.IsZoomAllAxes == true)
                    ////{
                    ////    this.EnableZooming = true;
                    ////}

                    ParentArea.HorizontalBar.ValueChanged += new PropertyChangedCallback(HorizontalBar_ValueChanged);
                    ParentArea.VerticalBar.ValueChanged += new PropertyChangedCallback(VerticalBar_ValueChanged);
                    //foreach (ChartSeries cs in ParentArea.Series)
                    //{
                    //    if (cs.InteractiveCursor != null)
                    //    {
                    //        if (cs.HorizontalCursor != null)
                    //        {
                    //            cs.SetValuesForCursor();
                    //        }
                    //    }
                    //}
                }
            }
            this.InvalidateMeasure();
            if (this.Area != null && this.Area.stripLinePanel != null && this.StripLines.Count > 0)
                this.Area.stripLinePanel.InvalidateMeasure();
        }
        private static void OnLabelVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                    axis.CursorLabelVisibility = axis.HorizontalLabelVisibility;
                else
                    axis.CursorLabelVisibility = axis.VerticalLabelVisibility;
            }
        }

        private static void OnLabelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                    axis.CursorLabelTemplate = axis.HorizontalLabelTemplate;
                else
                    axis.CursorLabelTemplate = axis.VerticalLabelTemplate;
            }
        }
        /// <summary>
        /// Zomming Vertical Scrollbar value changed.
        /// </summary>
        void VerticalBar_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (this.Orientation == Orientation.Vertical && this.ZoomFactor <1)
            {
                double zoomvalue = (this.Range.End - this.Range.Start) * this.ZoomFactor / 2;
                double verticalbarvalue = 1 - (double)e.NewValue;
                double actualscrollbarvalue = this.Range.Start + (verticalbarvalue * (this.Range.End - this.Range.Start));
                double start = actualscrollbarvalue - zoomvalue;
                double end = actualscrollbarvalue + zoomvalue;
                if (start < this.Range.Start)
                {
                    double diff = this.Range.Start - start;
                    start = this.Range.Start;
                    end += diff;
                }

                if (end > this.Range.End)
                {
                    double diff = end - this.Range.End;
                    end = this.Range.End;
                    start -= diff;
                }

                this.ZoomPosition = actualscrollbarvalue;
                if (this.ValueType != ChartValueType.DateTime && this.IsFractionEnabledOnZoom == true)
                {
                    this.ActualVisibleRange = new DoubleRange(Math.Round(start, 4), Math.Round(end, 4));
                }
                else if (this.ValueType == ChartValueType.DateTime && (this.IsFractionEnabledOnZoom == true || this.LabelsSource != null))
                {
                    this.ActualVisibleRange = new DoubleRange(start, end);
                }
                else
                {
                    this.ActualVisibleRange = new DoubleRange(Math.Ceiling(start), Math.Ceiling(end));
                }

                this.ZoomPosition = this.Range.End - (this.Range.Start + ((this.Range.End - this.Range.Start) * (double)e.NewValue));

                //foreach (ChartSeries series in this.axisBindedSeriesList)
                //{
                //    series.seriesGrid.RenderTransformOrigin = new Point(series.seriesGrid.RenderTransformOrigin.X, (double)e.NewValue);
                //}
            }
        }

        /// <summary>
        /// Zooming Horizontal Scroll bar value changed event
        /// </summary>
        void HorizontalBar_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (this.Orientation == Orientation.Horizontal && this.ZoomFactor < 1)
            {
                double zoomvalue = (this.Range.End - this.Range.Start) * this.ZoomFactor / 2;
                double actualscrollbarvalue = this.Range.Start + ((double)e.NewValue * (this.Range.End - this.Range.Start));
                double start = actualscrollbarvalue - zoomvalue;
                double end = actualscrollbarvalue + zoomvalue;
                if (start < this.Range.Start)
                {
                    double diff = this.Range.Start - start;
                    start = this.Range.Start;
                    end += diff;
                }

                if (end > this.Range.End)
                {
                    double diff = end - this.Range.End;
                    end = this.Range.End;
                    start -= diff;
                }

                this.ZoomPosition = actualscrollbarvalue;
                if (this.ValueType != ChartValueType.DateTime && (this.IsFractionEnabledOnZoom == true || this.LabelsSource != null))
                {
                    this.ActualVisibleRange = new DoubleRange(Math.Round(start, 4), Math.Round(end, 4));
                }
                else if (this.ValueType == ChartValueType.DateTime && (this.IsFractionEnabledOnZoom == true || this.LabelsSource != null))
                {
                    this.ActualVisibleRange = new DoubleRange(start, end);
                }
                else
                {
                    this.ActualVisibleRange = new DoubleRange(Math.Ceiling(start), Math.Ceiling(end));
                }


                this.ZoomPosition = this.Range.Start + ((this.Range.End - this.Range.Start) * (double)e.NewValue);

                //foreach (ChartSeries series in this.axisBindedSeriesList)
                //{
                //    series.seriesGrid.RenderTransformOrigin = new Point((double)e.NewValue, series.seriesGrid.RenderTransformOrigin.Y);
                //}
            }
        }

        /// <summary>
        /// Binding the Data from Labelsource
        /// </summary>
        public void BindDataFromDataSource()
        {
            if (this.LabelsSource != null && this.PositionPath != null && this.axisBindedSeriesList.Count>0)
            {
                //this.AxisDataContents = DataBinding.GetPropertyDataAsObject(this.LabelsSource, this.ContentPath);
                //this.AxisDataPosition = DataBinding.GetPropertyData(this.LabelsSource, this.PositionPath);
                this.AxisDataContents = this.axisBindedSeriesList[0].DataModel.GetAxisContents;
                this.AxisDataPosition = this.axisBindedSeriesList[0].DataModel.GetAxisPositions;
                double blank = 0d;
                if (AxisDataContents != null)// && this.axisBindedSeriesList[0].IsIndexed)
                {
                    this.AxisDataContents.Insert(0, "");
                }

                if (AxisDataPosition != null)
                {
                    this.AxisDataPosition.Insert(0, blank);
                    this.AxisDataPosition.Insert(this.AxisDataPosition.Count, blank);
                }
            }
        }

        /// <summary>
        /// Get parent area of current axis
        /// </summary>
        /// <returns>
        /// ChartArea Values
        /// </returns>
        internal ChartArea GetParentArea()
        {
            DependencyObject element = this;
            while (!(element is ChartArea) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as ChartArea;
            }

            return null;
        }

        internal ContentControl axisHeader = null;
        internal ItemsPresenter axisLabels = null;
        internal ChartAxisElementPanel axisElementPanel = null;
        TickLinesPanel tickLinesPanel = null;

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            if (this.Items.Count > 0 && axisElementPanel == null)
            {
                DataAxis axislabel = this.Items[0] as DataAxis;
                if (axislabel != null)
                {
                    axisElementPanel = VisualTreeHelper.GetParent(axislabel) as ChartAxisElementPanel;
                }
            }
            return finalSize;
        }

        /// <summary>
        /// call this method when Axis render on area
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            #region revamp
            this.GoToAxisVisualState();
            ModelBinding();
            if (this.EnableZooming)
            {
                this.ActualVisibleRange = this.Range;
                this.ActualVisibleInterval = this.VisibleInterval;
                if (this.ZoomFactor < 1)
                {
                    PerformZoomFactor(false);
                }
            }

            this.RefreshAxis();
            axisHeader = this.GetTemplateChild("axisHeader") as ContentControl;
            axisLabels = this.GetTemplateChild("axisLabels") as ItemsPresenter;
            tickLinesPanel = this.GetTemplateChild("tickLines") as TickLinesPanel;
            #endregion
            this.Axescontainer = GetTemplateChild("AxisContent") as ItemsControl;
        }

        void ModelBinding()
        {
            InitializeModel();
            this.ChartAxisModelBinding(this.Model, new PropertyPath("IsAutoSetRange"), BindingMode.TwoWay, this, ChartAxis.IsAutoSetRangeProperty);
            this.ChartAxisModelBinding(this.Model, new PropertyPath("Range"), BindingMode.TwoWay, this, ChartAxis.RangeProperty);
            this.ChartAxisModelBinding(this.Model, new PropertyPath("m_visibleInterval"), BindingMode.TwoWay, this, ChartAxis.VisibleIntervalProperty);
            this.ChartAxisModelBinding(this.Model, new PropertyPath("Header"), BindingMode.TwoWay, this, ChartAxis.HeaderProperty);
            this.ChartAxisModelBinding(this.Model, new PropertyPath("OpposedPosition"), BindingMode.TwoWay, this, ChartAxis.OpposedPositionProperty);
            this.ChartAxisModelBinding(this.Model, new PropertyPath("Orientation"), BindingMode.TwoWay, this, ChartAxis.OrientationProperty);
            this.ChartAxisModelBinding(this.Model, new PropertyPath("IntersectAction"), BindingMode.TwoWay, this, ChartAxis.IntersectActionProperty);
        }

        void InitializeModel()
        {
            this.Model.IsAutoSetRange = this.IsAutoSetRange;
            this.Model.Range = this.Range;
            this.Model.m_visibleInterval = this.VisibleInterval;
            this.Model.Header = this.Header;
            this.Model.OpposedPosition = this.OpposedPosition;
            this.Model.Orientation = this.Orientation;
            this.Model.IntersectAction = this.IntersectAction;
        }

        void ChartAxisModelBinding(object source, PropertyPath path, BindingMode mode, DependencyObject target, DependencyProperty targetProperty)
        {
            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = path;
            binding.Mode = mode;
            BindingOperations.SetBinding(target, targetProperty, binding);
        }

        internal bool isfirsttimeenabled = false;
        
        /// <summary>
        /// call this method when IsEnabledZoom property changed
        /// </summary>
        private static void OnEnableZoomingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            if (obj != null && obj.Area!=null)
            {
                obj.Area.IntializeZoomingScrollBarVisibility();
            }
        }

        private static void OnZoomPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                double ratiozoomposition = Math.Abs(axis.ZoomPosition - axis.Range.Start) / (axis.Range.End - axis.Range.Start);
                foreach (ChartSeries series in axis.axisBindedSeriesList)
                {
                    series.seriesGrid.RenderTransformOrigin = axis.Orientation == Orientation.Horizontal ? new Point(ratiozoomposition, series.seriesGrid.RenderTransformOrigin.Y) : new Point(series.seriesGrid.RenderTransformOrigin.X, 1 - ratiozoomposition);
                }
                if (axis.Area.ChartAreaParent != null)
                {
                    foreach (ChartSeries series in axis.Area.ChartAreaParent.Series)
                    {
                        series.seriesGrid.RenderTransformOrigin = axis.Orientation == Orientation.Horizontal ? new Point(ratiozoomposition, series.seriesGrid.RenderTransformOrigin.Y) : new Point(series.seriesGrid.RenderTransformOrigin.X, 1 - ratiozoomposition);
                    }
                }
            }
        }

        internal bool isupdatezoomfactor=false;


        internal void UpdateAdornmentOnZoom(ChartSeries series)
        {
            foreach (ContentPresenter segment in (from content in series.Presenter.Children.OfType<ContentPresenter>() where content.Content is ChartAdornment select content))
            {
                ContentPresenter presenter = segment as ContentPresenter;
                if (VisualTreeHelper.GetChildrenCount(presenter) > 0)
                {
                    Canvas canvas = VisualTreeHelper.GetChild(presenter, 0) as Canvas;
                    ChartAdornment adornment = presenter.Content as ChartAdornment;
                    if (canvas != null)
                    {
                        foreach (ContentControl element in canvas.Children)
                        {
                            if (element.RenderTransform is CompositeTransform)
                            {
                                (element.RenderTransform as CompositeTransform).ScaleX = 1d / series.ScaleX;
                                (element.RenderTransform as CompositeTransform).ScaleY = 1d / series.ScaleY;
                            }
                            else
                            {
                                element.RenderTransform = new ScaleTransform() { ScaleX = 1d / series.ScaleX, ScaleY = 1d / series.ScaleY };
                            }
                        }
                    }
                }
            }
        }

        void PerformZoomFactor(bool m_isPerformZoomfactor)
        {
            if (this.EnableZooming || m_isPerformZoomfactor)
            {
                double scale = 1d / (this.ZoomFactor <= 1 && !double.IsNaN(this.ZoomFactor) ? this.ZoomFactor : 1);
                scale = this.IsInversed ? (scale * -1) : scale;

                if (this.Area != null && this.Area.ChartAreaParent == null)
                {
                    foreach (ChartSeries series in this.axisBindedSeriesList)
                    {
                        series.ScaleX = this.Orientation == Orientation.Horizontal ? scale : series.ScaleX;
                        series.ScaleY = this.Orientation == Orientation.Vertical ? scale : series.ScaleY;
                        series.seriesGrid.RenderTransform = new ScaleTransform() { ScaleX = series.ScaleX, ScaleY = series.ScaleY };
                        UpdateAdornmentOnZoom(series);
                    }
                }
                else if (this.Area != null && this.Area.ChartAreaParent != null)
                {
                    foreach (ChartSeries series in this.Area.ChartAreaParent.Series)
                    {
                        series.ScaleX = this.Orientation == Orientation.Horizontal ? scale : series.ScaleX;
                        series.ScaleY = this.Orientation == Orientation.Vertical ? scale : series.ScaleY;
                        series.seriesGrid.RenderTransform = new ScaleTransform() { ScaleX = series.ScaleX, ScaleY = series.ScaleY };
                        UpdateAdornmentOnZoom(series);

                    }
 
                }

                //if (obj.isUpdateZoomrange == true)
                //{
                //    obj.isupdatezoomfactor = true;
                //}
                ChartArea area = this.Area == null ? this.ParentArea : this.Area;
                if (this.ZoomFactor >= 0.001 && this.ZoomFactor <= 1 && area != null)
                {
                    this.isUpdateViewportsize = true;
                    double start = this.Range.Start;
                    double end = this.Range.End;
                    if (double.IsNaN(this.ZoomPosition) == true)
                    {
                        this.ZoomPosition = this.ActualVisibleRange.Start + ((this.ActualVisibleRange.End - this.ActualVisibleRange.Start) / 2);
                    }

                    double visiblestart = this.ZoomPosition - ((end - start) * this.ZoomFactor / 2);
                    double visibleend = this.ZoomPosition + ((end - start) * this.ZoomFactor / 2);
                    if (visiblestart < start)
                    {
                        double diff = start - visiblestart;
                        visiblestart = start;
                        visibleend += diff;
                    }

                    if (visibleend > end)
                    {
                        double diff = visibleend - end;
                        visibleend = end;
                        visiblestart -= diff;
                    }

                    if (area.dragzoomrange.IsEmpty == false && this.LabelsSource != null)
                    {
                        //obj.ActualInterval = Math.Ceiling((obj.ZoomInterval / (obj.ZoomRange.End - obj.ZoomRange.Start)) * (visibleend - visiblestart));
                        this.ActualInterval = Math.Ceiling((this.VisibleInterval / (this.Range.End - this.Range.Start)) * (visibleend - visiblestart));
                    }
                    else
                    {
                        //obj.ActualInterval = (obj.ZoomInterval / (obj.ZoomRange.End - obj.ZoomRange.Start)) * (visibleend - visiblestart);
                        this.ActualInterval = (this.VisibleInterval / (this.Range.End - this.Range.Start)) * (visibleend - visiblestart);
                    }

                    this.PreviousRange = this.IsFractionEnabledOnZoom ? DoubleRange.Empty : this.ActualVisibleRange;
                    if (this.ValueType != ChartValueType.DateTime && (this.IsFractionEnabledOnZoom == true || this.LabelsSource != null))
                    {
                        this.ActualVisibleInterval = Math.Round(this.ActualInterval, 4);
                        this.ActualVisibleRange = new DoubleRange(Math.Round(visiblestart, 4), Math.Round(visibleend, 4));
                        
                    }
                    else if (this.ValueType == ChartValueType.DateTime && (this.IsFractionEnabledOnZoom == true || this.LabelsSource != null))
                    {
                        this.ActualVisibleInterval = this.IsAutoSetRange ? this.ActualInterval : this.VisibleInterval;
                        this.ActualVisibleRange = new DoubleRange(visiblestart, visibleend);
                        
                    }
                    else
                    {
                        this.ActualVisibleInterval = Math.Ceiling(this.ActualInterval);
                        this.ActualVisibleRange = new DoubleRange(Math.Ceiling(visiblestart), Math.Ceiling(visibleend));
                        
                    }


                    ZoomingScrollBar zoomscrollbar = this.Orientation == Orientation.Horizontal ? area.HorizontalBar : area.VerticalBar;
                    if (zoomscrollbar != null)
                    {
                        if (area.ChartAreaParent == null)
                        {
                            area.IntializeZoomingScrollBarVisibility();
                        }
                        else
                        {
                            area.ChartAreaParent.PrimaryAxis.ZoomFactor = this.ZoomFactor;
                            area.ChartAreaParent.IntializeZoomingScrollBarVisibility();
                        }

                        if (this.Orientation == Orientation.Vertical)
                            zoomscrollbar.Value = 1 - (1 / (this.Range.End - this.Range.Start) * (this.ZoomPosition - this.Range.Start));
                        else
                            zoomscrollbar.Value = 1 / (this.Range.End - this.Range.Start) * (this.ZoomPosition - this.Range.Start);

                        if (this.ZoomFactor == 1)
                        {
                            zoomscrollbar.ViewportSize = double.MaxValue;
                        }
                        else if (this.ZoomFactor >= 0.5 && this.isUpdateViewportsize == true)
                        {
                            zoomscrollbar.ViewportSize = this.ZoomFactor * 16;
                        }
                        else if (this.ZoomFactor < 0.5 && this.isUpdateViewportsize == true)
                        {
                            zoomscrollbar.ViewportSize = this.ZoomFactor * 4;
                        }
                    }
                }
            }
            if (this.EnableRangeSelectionOnMouseOver && this.Area!= null)
            {
                this.Area.RangeSelectionMouseOverHeight = 0d;
                this.Area.RangeSelectionMouseOverWidth = 0d;                
            }
            if (this.EnableRangeSelection && this.Area != null)
            {
                this.Area.RangeSelectionHeight = 0d;
                this.Area.RangeSelectionWidth = 0d;
                this.Area.CloseButtonVisibility = Visibility.Collapsed;
            }            
        }

        /// <summary>
        /// Calculate zooming value after Zoomfactor value changed
        /// </summary>
        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = (ChartAxis)d;
            if (axis != null && axis.EnableZooming)
            {
                axis.PerformZoomFactor(false);
            }
        }

        /// <summary>
        /// This method executes when axis RangeCalculationMode property value changed
        /// </summary>
        private static void OnRangePaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.VisibleRange = DoubleRange.Empty;
            obj.OnRangePaddingChanged(e);
        }
        /// <summary>
        /// Called when RangePadding changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRangePaddingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
            {
                this.Area.LoadArea();
            }
        }

        /// <summary>
        /// This method executes when axis RangeCalculationMode property value changed
        /// </summary>
        private static void OnRangeCalculationModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.VisibleRange = DoubleRange.Empty;
        }

        /// <summary>
        /// This method executes when axis EdgeLablesDrawingMode and HidePartialLabel property value changed
        /// </summary>
        private static void OnApperanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnApperanceChanged(e);
        }

        /// <summary>
        /// This method executes when axis EdgeLablesDrawingMode and HidePartialLabel property value changed
        /// </summary>
        protected virtual void OnApperanceChanged(DependencyPropertyChangedEventArgs e)
        {
            //if (this.Area != null)
            //{
            //    this.Area.LoadArea();
            //}

            this.RefreshAxis();
        }

        /// <summary>
        /// This method executes when axis LabelRotateAngle property value changed
        /// </summary>
        private static void OnRotateLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnRotateLabelChanged(e);
        }

        /// <summary>
        /// This method executes when axis LabelRotateAngle property value changed
        /// </summary>
        protected virtual void OnRotateLabelChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshAxis();
        }

        /// <summary>
        /// This method executes when axis IntersectAction property value changed
        /// </summary>
        private static void OnIntersectActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnIntersectActionChanged(e);
        }

        /// <summary>
        /// This method executes when axis IntersectAction property value changed
        /// </summary>
        protected virtual void OnIntersectActionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
            {
                this.maxrowsize = 0d;
                this.rotatemarginvalue = 0d;
                //this.Area.LoadArea();
            }

            if ((ChartLabelIntersectAction)e.NewValue != ChartLabelIntersectAction.Rotate)
            {
                this.LabelRotateAngle = 0;
                if (this.axisLabels != null)
                {
                    this.axisLabels.Width = this.axisLabels.Height = double.NaN;
                }
            }

            this.RefreshAxis();
        }

        /// <summary>
        /// This method executes when axis LabelFontSize, LabelFontStyle, LabelFontName property value changed
        /// </summary>
        private static void OnFontInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnFontInfoChanged(e);
        }

        /// <summary>
        /// This method executes when axis LabelFontSize, LabelFontStyle, LabelFontName property value changed
        /// </summary>
        protected virtual void OnFontInfoChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateAxis();
        }

        /// <summary>
        /// This method executes when axis ContentPath and PositionPath property value changed
        /// </summary>
        private static void OnBindingdataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.BindDataFromDataSource();
            obj.RefreshAxisContents(obj, e.Property);
            if (obj.ParentArea != null)
            {
                obj.ParentArea.LoadArea();
            }
        }

        void RefreshAxisContents(ChartAxis obj, DependencyProperty e)
        {
            if (obj != null && obj.axisBindedSeriesList.Count > 0)
            {
                foreach (ChartSeries series in obj.axisBindedSeriesList)
                {
                    if (series.DataModel != null)
                    {
                        if (e == ContentPathProperty)
                            series.DataModel.ContentPath = obj.ContentPath;
                        else if (e == PositionPathProperty)
                            series.DataModel.PositionPath = obj.PositionPath;
                    }
                }
            }
        }

        /// <summary>
        /// This method executes when axis AutosetRange property value changed
        /// </summary>
        private static void OnAutoSetRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnAutoSetRangeChanged(e);
        }

        /// <summary>
        /// This method executes when an IsAutoSetRange property value changed.
        /// </summary>
        protected virtual void OnAutoSetRangeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.AutoSetRangeChanged != null)
            {
                this.AutoSetRangeChanged(this, e);
            }

            this.VisibleRange = DoubleRange.Empty;
            this.DateTimeRange = new DateTimeRange(new DateTime(), new DateTime());

            this.RefreshAxis();
        }

        /// <summary>
        /// This method executes when OnDesiredIntervalsCount property changed
        /// </summary>
        protected virtual void OnDesiredIntervalsCountChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.DesiredIntervalsCountChanged != null)
            {
                this.DesiredIntervalsCountChanged(this, e);
            }

            if (this.Area != null)
            {
                this.Area.LoadArea();
            }
        }

        /// <summary>
        /// This method executes when DateTimeInterval property value changed
        /// </summary>
        private static void OnDateTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            if (obj.ValueType == ChartValueType.DateTime)
            {
                obj.iscalculateinterval = false;
                double resultinterval = (DateTime.Now + (TimeSpan)e.NewValue).ToOADate() - DateTime.Now.ToOADate();
                obj.IsTimeInterval = resultinterval < 1;
                if (obj.IsTimeInterval && obj.IsAutoSetRange)
                {
                    obj.Range = new DoubleRange(resultinterval, resultinterval * 2);
                }
                obj.VisibleInterval = resultinterval;
                ////obj.m_visibleInterval = Math.Round(obj.m_visibleInterval, 3);
            }
        }

        /// <summary>
        /// This method executes when DataTimeRange property value changed
        /// </summary>
        private static void OnDateTimeRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            if (obj.ValueType == ChartValueType.DateTime)
            {
                DateTimeRange dateTimeRange = (DateTimeRange)e.NewValue;
                obj.Range = new DoubleRange(dateTimeRange.Start.ToOADate(), dateTimeRange.End.ToOADate());
                if (obj.ParentArea != null)
                {
                    obj.ParentArea.LoadArea();
                }
            }
        }

        private static void OnLogRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = (ChartAxis)d;
            if (axis.ValueType == ChartValueType.Logarithmic || axis.IsLogarithmic)
            {
                DoubleRange logRange = (DoubleRange)e.NewValue;
                if (logRange.Start > 0 && logRange.End > 0)
                {
                    axis.Range = new DoubleRange(Math.Log(logRange.Start, axis.LogarithmicBase), Math.Log(logRange.End, axis.LogarithmicBase));
                    axis.ActualVisibleRange = new DoubleRange(Math.Log(logRange.Start, axis.LogarithmicBase), Math.Log(logRange.End, axis.LogarithmicBase));
                }
                else
                {
                    throw new InvalidOperationException("The Log Value Must be Greater than 0");
                }

                if (axis.ParentArea != null)
                {
                    axis.ParentArea.LoadArea();
                }

            }
        }

        /// <summary>
        /// This method executes when DesiredIntervalsCount property value changed
        /// </summary>
        private static void OnDesiredIntervalsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnDesiredIntervalsCountChanged(e);
        }

        /// <summary>
        /// This method executes when m_visibleInterval property value changed
        /// </summary>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnIntervalChanged(e);
        }

        private static void OnIntervalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                if(!double.IsNaN(axis.Interval) && axis.Interval != 0)
                {
                    axis.VisibleInterval = axis.Interval;
                }
            }
        }

        /// <summary>
        /// This method executes when Labelsource property changed
        /// </summary>
        private static void OnLabelSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartAxis obj = (ChartAxis)d;
            if (obj !=null && obj.InternalLabelsSourceSet == false)
            {
                obj.ActualLabelsSourceSet = true;
            }

            obj.InternalLabelsSourceSet = false;
            obj.populatechart((IEnumerable)args.NewValue);
            obj.BindDataFromDataSource();
            if (obj.ParentArea != null)
            {
                obj.ParentArea.LoadArea();
            }
        }

        /// <summary>
        /// This method populate chart when binding collection changed
        /// </summary>
        private IEnumerable Axiscoll;
        private void populatechart(IEnumerable newvalue)
        {
            if (newvalue is INotifyCollectionChanged)
            {
                Axiscoll = newvalue;
                ((INotifyCollectionChanged)Axiscoll).CollectionChanged += new NotifyCollectionChangedEventHandler(Axis_CollectionChanged);
            }
        }

        /// <summary>
        /// This method executes when Axes collection changed
        /// </summary>
        void Axis_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.LabelsSource = (IEnumerable)sender;
            this.BindDataFromDataSource();
            if (this.Area != null)
            {
                this.Area.LoadArea();
            }
        }

        /// <summary>
        /// This method executes when GridLineStroke, GridLineStrokeThickness, LineStroke, LineStrokeThickness property value changed
        /// </summary>
        private static void OnLineStyleTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.UpdateAxis();
            }
        }

        /// <summary>
        /// This method executes when m_visibleInterval property value changed
        /// </summary>
        protected virtual void OnIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IntervalChanged != null)
            {
                this.IntervalChanged(this, e);
            }

            if (double.IsNaN(this.userinterval))
            {
                this.userinterval = this.VisibleInterval;
            }

            //if (this.Area != null)
            //{
            //    this.Area.LoadArea();
            //}

            //this.RefreshAxis();

            if (this.EnableZooming == false || (this.ZoomFactor>=1))
            {
                this.ActualVisibleInterval = this.VisibleInterval;
            }
        }

        /// <summary>
        /// This method executes when GridLineStroke, GridLineStrokeThickness, LineStroke, LineStrokeThickness property value changed
        /// </summary>
        protected virtual void OnLineStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.LineStyleChanged != null)
            {
                this.LineStyleChanged(this, e);
            }


            if (this.Area != null && this.Area.gridLinePanel != null)
            {
                this.Area.gridLinePanel.InvalidateMeasure();
                if (this.Area.smallTickLinesPanel != null)
                {
                    if ((from d in this.Area.Axes select d.SmallTicksRequired).Contains<bool>(true))
                    {
                        this.Area.smallTickLinesPanel.Visibility = Visibility.Visible;
                        if (e.Property == ChartAxis.SmallTicksPerIntervalProperty)
                            this.Area.smallTickLinesPanel.RefreshPanel();
                        else
                            this.Area.smallTickLinesPanel.InvalidateMeasure();
                    }
                    else
                    {
                        this.Area.smallTickLinesPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }

            if (this.tickLinesPanel != null)
            {
                this.tickLinesPanel.InvalidateMeasure();
            }

            //if (this.ParentArea != null)
            //{
            //    this.ParentArea.LoadArea();
            //}
        }

        /// <summary>
        /// This method executes when GridLineStroke, GridLineStrokeThickness, LineStroke, LineStrokeThickness property value changed
        /// </summary>
        private static void OnLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnLineStyleChanged(e);
        }
        private static void OnLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            axis.GoToAxisVisualState();            
            axis.OnLineStyleChanged(e);
            axis.UpdateAxis();
        }

        /// <summary>
        /// This method executes when axis OpposedPosition property value changed
        /// </summary>
        private static void OnOpposedPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnOpposedPositionChanged(e);
        }

        /// <summary>
        /// This method executes when axis OpposedPosition property value changed
        /// </summary>
        protected virtual void OnOpposedPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.OpposedPositionChanged != null)
            {
                this.OpposedPositionChanged(this, e);
            }

            //if (this.Area != null)
            //{
            //    this.Area.LoadArea();
            //}

            this.UpdateAxis();
            this.GoToAxisVisualState();
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.RefreshAxis();
                axis.GoToAxisVisualState();
            }
        }

        internal bool iscalculateinterval = true;
        internal bool isUpdateActualVisibleRange = false;
        internal double CalculateNiceInterval(DoubleRange dr, int desiredIntervalCount)
        {
            double desiredInterval = dr.Delta / desiredIntervalCount;
            double mul = Math.Pow(LogarithmicBase, Math.Floor(Math.Log(desiredInterval, LogarithmicBase)));
            double minDelta = double.MaxValue;
            double minValue = double.MaxValue;

            double interval = 0d;

            //if (mul < offset)
            //{
            //    mul = 1;
            //}

            foreach (double div in c_intervalDivs)
            {
                double delta = Math.Abs(desiredInterval - div * mul);

                if (delta < minDelta)
                {
                    minDelta = delta;
                    interval = div * mul;
                }

                minValue = Math.Min(minValue, div);
            }

            if (Math.Abs(desiredInterval - minValue * LogarithmicBase * mul) < Math.Abs(desiredInterval - interval))
            {
                interval = minValue * LogarithmicBase * mul;
            }

            //if (this.Area.IsSync == true && this.ValueType != ChartValueType.DateTime)
            //{
            //    interval = ((dr.Start + dr.End) / (double)desiredIntervalCount);
            //    return interval;
            //}

            return interval;
        }
        /// <summary>
        /// This method executes when axis Range property value changed
        /// </summary>
        protected virtual void OnRangeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.RangeChanged != null)
            {
                this.RangeChanged(this, e);
            }

            ////&& this.ValueType!=ChartValueType.DateTime)
            if (this.IsAutoSetRange == true && this.iscalculateinterval==true)
            {
                this.userinterval = this.VisibleInterval;
                double start = this.Range.Start + (this.Range.Start * (-1));
                double end = this.Range.End + (this.Range.Start * (-1));
                if (double.IsNaN(this.Interval))
                {
                    double result = ((start + end) / (double)this.DesiredIntervalsCount);
                    result = this.CalculateNiceInterval(new DoubleRange(start, end), this.DesiredIntervalsCount);
                    result = result < 1 ? Math.Ceiling(result) : Math.Floor(result);
                    result = double.IsNaN(result) ? 1d : (this.IsLogarithmic ? Math.Ceiling(result) : result);
                    this.VisibleInterval = !this.IsFractionalData ? result : this.GetNiceInterval(this.Range, this.DesiredIntervalsCount);
                }
            }

            if (this.EnableZooming == false || isUpdateActualVisibleRange)
            {
                this.ActualVisibleRange = this.Range;
                this.isUpdateActualVisibleRange = false;
                if (this.ZoomFactor < 1 && this.EnableZooming)
                {
                    this.PerformZoomFactor(false);
                }
            }

            if (this.Area != null)
            {
                //this.RefreshAxis();
                this.Area.LoadArea();
            }
        }

        private static void OnAxesProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.GoToAxisVisualState();
                axis.RefreshAxis();
            }
        }

        private static void OnActualVisibleIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.RefreshAxis();

                if (axis.tickLinesPanel != null)
                {
                    axis.tickLinesPanel.InvalidateMeasure();
                }
            }
        }

        private static void OnActualVisibleRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null)
            {
                axis.RefreshAxis();

                if (axis.tickLinesPanel != null)
                {
                    axis.tickLinesPanel.InvalidateMeasure();
                }
            }
        }
        /// <summary>
        /// This method executes when axis Range property value changed
        /// </summary>
        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis obj = (ChartAxis)d;
            obj.OnRangeChanged(e);

            if (obj.ParentArea != null && obj.IsAutoSetRange == false)
            {
                obj.ParentArea.LoadArea();
            }            
            
        }

        /// <summary>
        /// Format the Axis labels specify in LabelFormat or LabelDateTimeFormat property
        /// </summary>
        /// <returns>
        /// The Formatted string value
        /// </returns>
        internal string GetFormattedAxisLabel(string axisLabel)
        {
            string formattedAxisLabel = string.Empty;
            double val=0;
            DateTime date;
            bool canConvertDouble = double.TryParse(axisLabel, out val);
            bool canConvertDateTime = DateTime.TryParse(axisLabel, out date);

            if (canConvertDouble && (this.ValueType == ChartValueType.Logarithmic || this.IsLogarithmic))
            {
                formattedAxisLabel = Math.Pow(this.LogarithmicBase, val).ToString(this.LabelFormat,System.Globalization.CultureInfo.CurrentUICulture);
                return formattedAxisLabel;
            }

            ChartValueType compareValueType;
            if (LabelsSource != null && ContentPath != null)
            {
                Type labelType = DataBinding.GetPropertyType(this.LabelsSource, this.ContentPath);
                if (labelType == null && this.ValueType == ChartValueType.DateTime)
                {
                    labelType = typeof(DateTime);
                }

                if (labelType == typeof(string))
                {
                    ActualValueType = ChartValueType.String;
                    canConvertDouble = false;
                    canConvertDateTime = false;
                }
                else if (labelType == typeof(DateTime) || labelType == typeof(DateTime?))
                {
                    ActualValueType = ChartValueType.DateTime;
                }
                else if (labelType == typeof(double))
                {
                    ActualValueType = ChartValueType.Double;
                }
                else if (this.ValueType == ChartValueType.DateTime && (canConvertDouble || canConvertDateTime))
                {
                    ActualValueType = ChartValueType.DateTime;
                }
                else if (labelType == typeof(object))
                {
                    return null;
                }
                
             }

            compareValueType = ActualValueType;

            if ((compareValueType == ChartValueType.DateTime && ValueType == ChartValueType.Double) ||
                (compareValueType==ChartValueType.Double && ValueType==ChartValueType.DateTime))
            {
                compareValueType = ChartValueType.Double;
            }

            if (canConvertDouble && compareValueType == ChartValueType.Double)
            {
                if (this.LabelFormat != string.Empty)
                {
                    try
                    {
                        formattedAxisLabel = val.ToString(this.LabelFormat,System.Globalization.CultureInfo.CurrentUICulture);
                    }
                    catch
                    {
                        formattedAxisLabel = axisLabel;
                    }
                }
                else
                {
                    formattedAxisLabel = axisLabel;
                }
            }
            else if ((canConvertDateTime || canConvertDouble) && compareValueType == ChartValueType.DateTime)
            {
                if (canConvertDouble)
                {
                    date = DateTime.FromOADate(val);
                }

                if (this.LabelDateTimeFormat != string.Empty)
                {
                    try
                    {
                        formattedAxisLabel = date.ToString(this.LabelDateTimeFormat);
                    }
                    catch
                    {
                        formattedAxisLabel = date.ToString("MM/dd/yyyy");
                    }
                }
                else
                {
                    formattedAxisLabel = date.ToString("MM/dd/yyyy");
                }
            }
            else if (compareValueType == ChartValueType.String && !(canConvertDouble || canConvertDateTime))
            {
                formattedAxisLabel = axisLabel;
            }

            if (formattedAxisLabel == string.Empty)
                return null;
            else if (this.ValueType == ChartValueType.DateTime && canConvertDouble)
            {
                return DateTime.FromOADate(val).ToString(this.LabelDateTimeFormat);
            }
            else
                return formattedAxisLabel;
        }

        internal double GetNiceInterval(DoubleRange dr, int desiredIntervalCount)
        {
            double[] c_intervalDivs = new double[] { 1d, 2d, 3d, 5d };
            double desiredInterval = dr.Delta / desiredIntervalCount;
            double mul = Math.Pow(10, Math.Floor(Math.Log(desiredInterval, 10)));
            double minDelta = double.MaxValue;
            double minValue = double.MaxValue;

            double interval = 1d;

            foreach (double div in c_intervalDivs)
            {
                double delta = Math.Abs(desiredInterval - div * mul);

                if (delta < minDelta)
                {
                    minDelta = delta;
                    interval = div * mul;
                }

                minValue = Math.Min(minValue, div);
            }

            if (Math.Abs(desiredInterval - minValue * 10 * mul) < Math.Abs(desiredInterval - interval))
            {
                interval = minValue * 10 * mul;
            }

            return interval;
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Loaded -= new RoutedEventHandler(this.Axis_Loaded);
            if (Axiscoll != null)
                ((INotifyCollectionChanged)Axiscoll).CollectionChanged -= new NotifyCollectionChangedEventHandler(Axis_CollectionChanged);
            this.AutoSetRangeChanged = null;
            this.DesiredIntervalsCountChanged = null;
            this.IntervalChanged = null;
            this.LineStyleChanged = null;
            this.RangeChanged = null;
            this.OpposedPositionChanged = null;
            this.SelectedRangeChanged = null;

            this.ClearValue(ChartAxis.IntersectActionProperty);
            this.ClearValue(ChartAxis.RangeProperty);
            this.ClearValue(ChartAxis.VisibleRangeProperty);
            this.ClearValue(ChartAxis.DateTimeRangeProperty);
            this.ClearValue(ChartAxis.RangeCalculationModeProperty);
            this.ClearValue(ChartAxis.AxisLabelsProperty);

            if (this.AxisDataContents != null)
            {
                this.AxisDataContents.Clear();
                this.AxisDataContents = null;
            }

            if (this.AxisDataPosition != null)
            {
                this.AxisDataPosition.Clear();
                this.AxisDataPosition = null;
            }

            if (this.IndexDataContents != null)
            {
                this.IndexDataContents.Clear();
                this.IndexDataContents = null;
            }

            if (this.Area != null)
                this.Area = null;
            if (m_area != null)
            {
                m_area.InteractiveCursors.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(m_area.InteractiveCursorCollections_CollectionChanged);
                m_area.InteractiveCursors.Clear();
                m_area.InteractiveCursors = null;
                m_area = null;
            }
            
            if (ParentArea != null)
            {
                if (ParentArea.HorizontalBar != null && ParentArea.VerticalBar!=null)
                {
                    ParentArea.HorizontalBar.ValueChanged -= new PropertyChangedCallback(HorizontalBar_ValueChanged);
                    ParentArea.VerticalBar.ValueChanged -= new PropertyChangedCallback(VerticalBar_ValueChanged);
                }
                this.ParentArea = null;
            }

            if (this.axisvaluecontainer != null)
            {
                //for (int temp = 0; temp < this.axisvaluecontainer.Count; temp++)
                //{
                //    this.axisvaluecontainer[temp].Content = null;
                //    this.axisvaluecontainer[temp].ContentTemplate = null;
                //    this.axisvaluecontainer[temp] = null;
                //}
                this.axisvaluecontainer.Clear();
                this.axisvaluecontainer = null;
            }

            if (this.Axescontainer != null)
            {
                this.Axescontainer.Items.Clear();
                this.Axescontainer = null;
            }

            if (this.StripLines != null)
            {
                this.StripLines.CollectionChanged -= new NotifyCollectionChangedEventHandler(StripLines_CollectionChanged);
                this.StripLines.Clear();
                this.m_stripLines = null;
            }
            this.m_stripLines = null;
            this.axisElementPanel = null;
            this.ClearValue(RangeCalculationModeProperty);
            this.Resources.Clear();
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Class implementation for ChartAxislabel
    /// </summary>
    public class ChartAxisLabel : IDisposable
    {
        #region Members
        /// <summary>
        /// Initializes m_position
        /// </summary>
        private double m_position = double.NaN;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the position of label on axis.
        /// </summary>
        /// <value>The position.</value>
        public double Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        /// <summary>
        /// Gets or sets the content that label should display.
        /// </summary>
        /// <value>Label's content.</value>
        public object Content
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisLabel"/> class.
        /// </summary>
        public ChartAxisLabel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisLabel"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        public ChartAxisLabel(double position)
            : this(position, position)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisLabel"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="label">The label.</param>
        public ChartAxisLabel(double position, object label)
        {
            m_position = position;
            this.Content = label;
        }
        #endregion

        #region Public methdos
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current axis label.
        /// </returns>
        public override string ToString()
        {
            return this.Content.ToString();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ////Content = null;
        }

        #endregion
    }
}

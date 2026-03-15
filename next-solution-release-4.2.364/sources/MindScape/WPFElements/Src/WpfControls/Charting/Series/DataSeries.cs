using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a data series that can be plotted by a <see cref="Chart"/> control.
  /// </summary>
  public abstract class DataSeries : Control
  {
    private Canvas _canvas;
    private Canvas _foregroundCanvas;
    private ChartAxis _xAxis, _yAxis;
    private Chart _chart;

    /// <summary>
    /// Called when a dependency property value changes.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property == DataSeries.VisibilityProperty)
      {
        RequestAnalyseAndRebuildAll();
      }
    }

    private LegendItem _legendItem; // TODO: Rather than putting the logic in the LegendItems property, there should be overridable methods to update the legend icons. This will improve performance (and the code) as currently we update ALL legend items of ALL series whenever one thing changes.

    /// <summary>
    /// Gets a list of legend items to be displayed in the legend for this <see cref="DataSeries"/>.
    /// </summary>
    public virtual IList<LegendItem> LegendItems
    {
      get
      {
        IList<LegendItem> items = new List<LegendItem>();
        if (IsShownInLegend)
        {
          _legendItem = new LegendItem(Title, LegendIconTemplate, SeriesBrush, this);
          items.Add(_legendItem);
        }
        return items;
      }
    }

    #region LegendIconTemplate property

    /// <summary>
    /// Gets or sets the LegendIconTemplate.
    /// This is a dependency property.
    /// </summary>
    public DataTemplate LegendIconTemplate
    {
      get { return (DataTemplate)GetValue(LegendIconTemplateProperty); }
      set { SetValue(LegendIconTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LegendIconTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty LegendIconTemplateProperty =
      DependencyProperty.Register("LegendIconTemplate", typeof(DataTemplate), typeof(DataSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnLegendIconTemplateChanged)));

    private static void OnLegendIconTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnLegendIconTemplateChanged();
    }

    internal virtual void OnLegendIconTemplateChanged()
    {
      //OnLegendItemsChanged();
      if (_legendItem != null)
      {
        _legendItem.LegendIconTemplate = LegendIconTemplate;
      }
    }

    #endregion // LegendIconTemplate property

    #region ItemsSource property

    /// <summary>
    /// Gets or sets the ItemsSource. This is the data to be plotted on a <see cref="Chart"/> control.
    /// This is a dependency property.
    /// </summary>
    public IList ItemsSource
    {
      get { return (IList)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register("ItemsSource", typeof(IList), typeof(DataSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnItemsSourceChanged)));

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnItemsSourceChanged(e);
    }

    private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
    {
      UpdatePropertyInfo();
      SelectedDataPoint = null;
      //_dataPointMap = new List<CartesianDataPoint>();
      /*if (e.NewValue != null)
      {
        // TODO: this is bound to be killing performance. Maybe this can be done in the analyse series method? or only update it when data points are being requested.
        for (int i = 0; i < ((IList)e.NewValue).Count; i++)
        {
          _dataPointMap.Add(null);
        }
      }*/
      _isMinMaxCacheDirty = true;
      if (e.OldValue != null)
      {
        if (XAxis != null)
        {
          if (XAxis.LabelMap != null)
          {
            XAxis.LabelMap.Clear();
          }
          XAxis.DataMap.Clear();
        }
        if (YAxis != null)
        {
          if (YAxis.LabelMap != null)
          {
            YAxis.LabelMap.Clear();
          }
          YAxis.DataMap.Clear();
        }

        INotifyCollectionChanged collection = e.OldValue as INotifyCollectionChanged;
        if (collection != null)
        {
          collection.CollectionChanged -= new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
        }
        else
        {
          IBindingList bindingList = e.OldValue as IBindingList;
          if (bindingList != null)
          {
            bindingList.ListChanged -= new ListChangedEventHandler(BindingList_ListChanged);
          }
        }
        //RequestAnalyseAndRebuildAll();
      }
      else
      {
        //if (ItemsSource.Count > 0)
        //{
          // TODO: don't need this code. But the test environment needs this.
          //AnalyseSeries();
        //}
        if (Canvas != null && XAxis != null && YAxis != null)
        {
          //BuildChart();// TODO: don't need this code. But the test environment needs this.
          // TODO: this next line is to update the labels if this series has a custom object mapping.
          // Would be nice if we didn't need to call a ChartAxis method from a DataSeries.
          //XAxis.OnPositionsChanged();// TODO: don't need this code. But the test environment needs this. This may be solved by not looking at the Canvas elements in the tests.

          OnLegendItemsChanged(); // TODO I don't think this needs to be inside this condition.
        }
      }
      INotifyCollectionChanged notifyingCollection = ItemsSource as INotifyCollectionChanged;
      if (notifyingCollection != null)
      {
        notifyingCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
      }
      else
      {
        IBindingList bindingList = ItemsSource as IBindingList;
        if (bindingList != null)
        {
          bindingList.ListChanged += new ListChangedEventHandler(BindingList_ListChanged);
        }
      }

      //if (IsLoaded == null)
      {
        RequestAnalyseAndRebuildAll();
      }
    }

    private void BindingList_ListChanged(object sender, ListChangedEventArgs e)
    {
      RequestAnalyseAndRebuildAll();
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UpdatePropertyInfo();
      _isMinMaxCacheDirty = true;
      if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        for (int i = 0; i < e.OldItems.Count; i++)
        {
          // TODO: what to do when the selected data point is removed?
          /*if (_dataPointMap[e.OldStartingIndex] == SelectedDataPoint)
          {
            SelectedDataPoint = null;
          }*/
          //_dataPointMap.RemoveAt(e.OldStartingIndex);
          DataPointRemoved(e.OldItems[i], e.OldStartingIndex + i);
        }
      }
      else if (e.Action == NotifyCollectionChangedAction.Add)
      {
        for (int i = 0; i < e.NewItems.Count; i++)
        {
          //_dataPointMap.Insert(e.NewStartingIndex, null);
          DataPointAdded(e.NewItems[i], e.NewStartingIndex + i);
        }
      }
      else if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        /*_dataPointMap = new List<CartesianDataPoint>();
        for (int i = 0; i < ItemsSource.Count; i++)
        {
          _dataPointMap.Add(null);
        }*/
        ItemsSourceResetCore();
        SelectedDataPoint = null;
        RequestAnalyseAndRebuildAll();
      }
    }

    internal virtual void DataPointAddedCore(object dataPoint, int index) { }
    internal virtual void DataPointRemovedCore(object dataPoint, int index) { }
    internal virtual void ItemsSourceResetCore() { }

    // TODO: these methods do not yet support stacked series or series with more than 2 data values.

    // TODO: Test this method!
    private void DataPointAdded(object dataPoint, int index)
    {
      Point point = GetPoint(dataPoint, index);
      if (!Double.IsNaN(point.X))
      {
        IndependentMinimum = Math.Min(IndependentMinimum, point.X);
        IndependentMaximum = Math.Max(IndependentMaximum, point.X);
      }
      if (!Double.IsNaN(point.Y))
      {
        DependentMinimum = Math.Min(DependentMinimum, point.Y);
        DependentMaximum = Math.Max(DependentMaximum, point.Y);
      }


      if (ItemsSource.Count < 2)
      {
        MinDelta = 1;
      }
      if (index > 0)
      {
        object previousDataPoint = ItemsSource[index - 1];
        Point previousPoint = GetPoint(previousDataPoint, index - 1);
        SetMinDelta(Math.Min(MinDelta == 0 ? Double.MaxValue : MinDelta, Math.Abs(point.X - previousPoint.X)));
      }
      // TODO: also check next data point.

      // TODO: check for smaller min delta
      // TODO: update index of smallest/largest data point.
      // TODO: check the data is still ordered.

      // TODO: update _missingIndexCache

      DataPointAddedCore(dataPoint, index);

      OnUpdateAxes();
    }

    // TODO: Test this method!
    private void DataPointRemoved(object dataPoint, int index)
    {
      Point point = GetPoint(dataPoint, index);
      if (ReverseAxes)
      {
        // TODO: use the same improvements seen below when ReverseAxis is false.
        if (point.X == IndependentMinimum || point.X == IndependentMaximum || point.Y == DependentMinimum || point.Y == DependentMaximum)
        {
          Dispatcher.BeginInvoke(new Action(RequestAnalyseAndRebuildAll), DispatcherPriority.ApplicationIdle);
        }
      }
      else
      {
        if (IsDataOrdered)
        {
          if (point.X == IndependentMinimum && ItemsSource.Count > 0)
          {
            object firstDataPoint = ItemsSource[0];
            Point firstPoint = GetPoint(firstDataPoint, 0);
            IndependentMinimum = firstPoint.X;
          }
          else if (point.X == IndependentMaximum && ItemsSource.Count > 0)
          {
            object lastDataPoint = ItemsSource[ItemsSource.Count - 1];
            Point lastPoint = GetPoint(lastDataPoint, ItemsSource.Count - 1);
            IndependentMaximum = lastPoint.X;
          }
          if (point.Y == DependentMinimum || point.Y == DependentMaximum)
          {
            Dispatcher.BeginInvoke(new Action(RequestAnalyseAndRebuildAll), DispatcherPriority.ApplicationIdle);
          }
        }
        else if (point.X == IndependentMinimum || point.X == IndependentMaximum || point.Y == DependentMinimum || point.Y == DependentMaximum)
        {
          Dispatcher.BeginInvoke(new Action(RequestAnalyseAndRebuildAll), DispatcherPriority.ApplicationIdle);
        }
      }

      // TODO: update min delta
      // TODO: update index of smallest/largest data point.

      // NOTE: removing data can not cause it to become unordered.
      // TODO: if the data was not ordered before, check that it is ordered now. But how to do this fast?

      // TODO: update _missingIndexCache

      DataPointRemovedCore(dataPoint, index);

      OnUpdateAxes();
    }

    internal event EventHandler UpdateAxisRanges;

    // This is internal so it can be called by scatter series.
    internal void OnUpdateAxes()
    {
      EventHandler handler = UpdateAxisRanges;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
      RequestRebuild();
    }

    internal event EventHandler AnalyseAndRebuildAll;

    // Forces the parent Chart control to analyse and rebuild all the data series.
    internal void RequestAnalyseAndRebuildAll()
    {
      EventHandler handler = AnalyseAndRebuildAll;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler Rebuild;

    // Forces the chart control to re-render all the data series.
    internal void RequestRebuild()
    {
      EventHandler handler = Rebuild;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    // This list contains the index of the non-null points that surround a group of 1 or more null points. Also includes exactly 1 null point in a group of null points.
    private readonly IList<int> _missingIndexCache = new List<int>();

    internal virtual void AnalyseSeries()
    {
      // TODO: test various scenarios involving NaN, MaxValue and Infinite values.
      _isMinMaxCacheDirty = true;
      _missingIndexCache.Clear();
      if (ItemsSource != null && ItemsSource.Count > 0 && (XAxis != null || YAxis != null))
      {
        IAxisValueConverter xConverter = XAxis == null ? null : XAxis.ValueConverter;
        IAxisValueConverter yConverter = YAxis == null ? null : YAxis.ValueConverter;
        bool hasConverter = xConverter != null || yConverter != null;
        bool hasBinding = XBinding != null || YBinding != null;
        double independentMinimum = Double.MaxValue;
        double independentMaximum = Double.MinValue;
        double dependentMinimum = Double.MaxValue;
        double dependentMaximum = Double.MinValue;
        int count = 0;
        Point? previousPoint = null;
        Point? previousNullablePoint = new Point();
        double minDelta = Double.MaxValue;
        IsDataOrdered = true;
        bool isAllXNaN = true; // TODO: this may be impacting load performance.
        bool isAllYNaN = true;

        foreach (object o in ItemsSource)
        {
          if (o != null)
          {
            Point point = (!hasBinding && !hasConverter && o is Point) ? (Point)o : GetPoint(o, count);
            if (ReverseAxes)
            {
              point = new Point(point.Y, point.X);
            }

            double dependentLow;
            double dependentHigh;
            GetDataPointDimensions(o, point, out dependentLow, out dependentHigh);

            if (!Double.IsNaN(point.X))
            {
              isAllXNaN = false;
              // Minimum independent value
              independentMinimum = Math.Min(point.X, independentMinimum);
              // Maximum independent value
              independentMaximum = Math.Max(point.X, independentMaximum);
            }
            if (!Double.IsNaN(dependentLow))
            {
              isAllYNaN = false;
              // Minimum dependent value
              if (dependentLow < dependentMinimum)
              {
                dependentMinimum = dependentLow;
              }
            }
            if (!Double.IsNaN(dependentHigh))
            {
              isAllYNaN = false;
              // Maximum dependent value
              if (dependentHigh > dependentMaximum)
              {
                dependentMaximum = dependentHigh;
              }
            }
            if (!Double.IsNaN(point.X))
            {
              // min delta calculation
              if (previousPoint != null)
              {
                minDelta = Math.Min(minDelta, Math.Abs(point.X - previousPoint.Value.X));
              }
            }
            // Is data ordered
            if (IsDataOrdered && !Double.IsNaN(point.X) && previousPoint != null && !Double.IsNaN(previousPoint.Value.X))
            {
              if (point.X < previousPoint.Value.X)
              {
                IsDataOrdered = false;
              }
            }

            // Check if 'missing' data point
            if (Double.IsNaN(point.X) || Double.IsNaN(point.Y))
            {
              if (previousNullablePoint != null)
              {
                if (count > 0)
                {
                  if (_missingIndexCache.Count == 0 || _missingIndexCache[_missingIndexCache.Count - 1] != (count - 1))
                  {
                    _missingIndexCache.Add(count - 1);
                  }
                }
                _missingIndexCache.Add(count);
              }
              previousNullablePoint = null;
            }
            else
            {
              if (previousNullablePoint == null)
              {
                _missingIndexCache.Add(count);
              }
              previousNullablePoint = point;
            }

            previousPoint = point;
          }
          else
          {
            if (previousNullablePoint != null)
            {
              if (count > 0)
              {
                if (_missingIndexCache.Count == 0 || _missingIndexCache[_missingIndexCache.Count - 1] != (count - 1))
                {
                  _missingIndexCache.Add(count - 1);
                }
              }
              _missingIndexCache.Add(count);
            }
            previousNullablePoint = null;
          }
          count++;
        }

        if (isAllXNaN)
        {
          independentMinimum = 0;
          independentMaximum = 0;
        }
        if (isAllYNaN)
        {
          dependentMinimum = 0;
          dependentMaximum = 0;
        }

        IndependentMinimum = independentMinimum;
        IndependentMaximum = independentMaximum;
        DependentMinimum = dependentMinimum;
        DependentMaximum = dependentMaximum;

        SetMinDelta(minDelta);
      }
      else
      {
        IndependentMinimum = Double.MaxValue;
        IndependentMaximum = Double.MinValue;
        DependentMinimum = Double.MaxValue;
        DependentMaximum = Double.MinValue;
      }
    }

    // Gets the low and high values extracted from the given data object against the dependent axis.
    // The given point is the standard logical data extracted from the given data object.
    // point.X is the independent value, point.Y is the dependent value.
    internal virtual void GetDataPointDimensions(object dataPoint, Point point, out double dependentLow, out double dependentHigh)
    {
      dependentLow = point.Y;
      dependentHigh = point.Y;
    }

    private void SetMinDelta(double minDelta)
    {
      if (minDelta == Double.MaxValue)
      {
        ChartAxis axis = XAxis;
        if (ReverseAxes)
        {
          axis = YAxis;
        }
        if (!Double.IsNaN(axis.MaximumValue) && !Double.IsNaN(axis.MinimumValue))
        {
          minDelta = axis.ActualMajorTickMarkSpacing;
        }
        else
        {
          minDelta = 1;
        }
      }
      if (minDelta == 0 || Double.IsNaN(minDelta))
      {
        minDelta = 1;
      }
      MinDelta = minDelta;
    }

    internal double IndependentMinimum { get; set; }

    internal double IndependentMaximum { get; set; }

    internal double DependentMinimum { get; set; }

    internal double DependentMaximum { get; set; }

    internal virtual bool ReverseAxes { get { return false; } }

    internal virtual bool AlwaysPlotLastDataPoint { get { return true; } }

    #endregion // ItemsSource property

    // gets the minimum logical spacing between adjacent data points. This is useful for calculating the width of bars in a bar chart.
    internal double MinDelta { get; set; }

    // TODO: Should AlwaysShowYAxisZero and YAxisDataBuffer be moved to ChartAxis in the next major build??

    #region AlwaysShowYAxisZero Property

    /// <summary>
    /// Gets or sets whether or not the Y axis will always include the zero coordinate when the range is automatically being set.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AlwaysShowYAxisZeroProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AlwaysShowYAxisZero
    {
      get { return (bool)GetValue(AlwaysShowYAxisZeroProperty); }
      set { SetValue(AlwaysShowYAxisZeroProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AlwaysShowYAxisZero"/> property.
    /// </summary>
    public static readonly DependencyProperty AlwaysShowYAxisZeroProperty =
      DependencyProperty.Register("AlwaysShowYAxisZero", typeof(bool), typeof(DataSeries),
      new FrameworkPropertyMetadata(true, OnAlwaysShowYAxisZeroChanged));

    private static void OnAlwaysShowYAxisZeroChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnAlwaysShowYAxisZeroChanged();
    }

    private void OnAlwaysShowYAxisZeroChanged()
    {
      // TODO: need to auto calculate axis range.
    }

    #endregion // AlwaysShowYAxisZero Property

    #region YAxisDataBuffer Property

    /// <summary>
    /// Gets or sets a logical data buffer for automatically setting the minimum and maximum Y axis values.
    /// The default is 1. The data buffer is the logical gap between the largest/smallest data point and the calculated limits of the Y axis.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="YAxisDataBufferProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double YAxisDataBuffer
    {
      get { return (double)GetValue(YAxisDataBufferProperty); }
      set { SetValue(YAxisDataBufferProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YAxisDataBuffer"/> property.
    /// </summary>
    public static readonly DependencyProperty YAxisDataBufferProperty =
      DependencyProperty.Register("YAxisDataBuffer", typeof(double), typeof(DataSeries),
      new FrameworkPropertyMetadata(1.0, OnYAxisDataBufferChanged));

    private static void OnYAxisDataBufferChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnYAxisDataBufferChanged();
    }

    private void OnYAxisDataBufferChanged()
    {
      // TODO: need to auto calculate axis range.
    }

    #endregion // YAxisDataBuffer Property

    internal Point GetPoint(int index)
    {
      Point? point = null;
      if (ItemsSource != null && index < ItemsSource.Count)
      {
        point = GetPoint(ItemsSource[index], index);
      }
      return point == null ? new Point() : (Point)point;
    }

    private readonly ValueExtractor _xExtractor = new ValueExtractor() { DataContext = null };
    private readonly ValueExtractor _yExtractor = new ValueExtractor() { DataContext = null };

    internal Point GetPoint(object data, int index)
    {
      object xObject, yObject;
      return GetPoint(data, index, out xObject, out yObject);
    }

    internal Point GetPoint(object data, int index, out object xObject, out object yObject)
    {
      xObject = null;
      yObject = null;

      // Null check
      if (data == null)
      {
        return new Point(Double.NaN, Double.NaN);
      }

      // Check for built in data point types
      if ((XBinding == null || YBinding == null) && DataExtractor == null)
      {
        CheckBuiltInPointData(data, out xObject, out yObject);
      }

      // Use data extractor if it exists.
      if (DataExtractor != null)
      {
        xObject = DataExtractor.GetX(data);
      }
      else if (XBinding != null) // Use X binding if it exists
      {
        if (_xPropertyInfo == null)
        {
          UpdatePropertyInfo(data); // Because the tests bypass the property info cache.
        }
        if (_xPropertyInfo != null)
        {
          xObject = _xPropertyInfo.GetValue(data, null);
        }
        //_xExtractor.DataContext = data;
        //xObject = _xExtractor.Value;
      }
      else if (xObject == null)
      {
        // primitive data support
        xObject = ReverseAxes ? data : index;
      }

      // Use X axis to get logical position
      double x = 0;
      if (XAxis != null)
      {
        x = XAxis.GetLogicalPosition(xObject);
      }
      else
      {
        // This is mostly used for the testing environment when there is no X axis
        double? temp = NumericalUtils.ConvertToDouble(xObject);
        if (temp != null)
        {
          x = temp.Value;
        }
      }

      // Use data extractor if it exists.
      if (DataExtractor != null)
      {
        yObject = DataExtractor.GetY(data);
      }
      if (YBinding != null) // Use the Y binding if it exists
      {
        if (_yPropertyInfo == null)
        {
          UpdatePropertyInfo(data); // Because the tests bypass the property info cache.
        }
        if (_yPropertyInfo != null)
        {
          yObject = _yPropertyInfo.GetValue(data, null);
        }
        //_yExtractor.DataContext = data;
        //yObject = _yExtractor.Value;
      }
      else if (yObject == null)
      {
        // primitive data support
        yObject = ReverseAxes ? index : data;
      }

      double y = 0;
      if (YAxis != null)
      {
        // Use Y axis to get logical position
        y = YAxis.GetLogicalPosition(yObject);
      }
      else
      {
        // This is mostly used for the testing environment when there is no Y axis
        double? temp = NumericalUtils.ConvertToDouble(yObject);
        if (temp != null)
        {
          y = temp.Value;
        }
      }

      return new Point(x, y);
    }

    private void CheckBuiltInPointData(object data, out object xObject, out object yObject)
    {
      if (data is Point)
      {
        Point point = (Point)data;
        xObject = point.X;
        yObject = ((Point)data).Y;
      }
      else if (data is StringDouble)
      {
        StringDouble stringDouble = (StringDouble)data;
        if (ReverseAxes)
        {
          xObject = stringDouble.Double;
          yObject = stringDouble.String;
        }
        else
        {
          xObject = stringDouble.String;
          yObject = stringDouble.Double;
        }
      }
      else if (data is DateTimeDouble)
      {
        DateTimeDouble dateTimeDouble = (DateTimeDouble)data;
        if (ReverseAxes)
        {
          xObject = dateTimeDouble.Double;
          yObject = dateTimeDouble.DateTime;
        }
        else
        {
          xObject = dateTimeDouble.DateTime;
          yObject = dateTimeDouble.Double;
        }
      }
      else if (data is Point3)
      {
        Point3 point3 = (Point3)data;
        xObject = point3.X;
        yObject = point3.Y;
      }
      else if (data is StringDoubleDouble)
      {
        StringDoubleDouble stringDoubleDouble = (StringDoubleDouble)data;
        xObject = stringDoubleDouble.String;
        yObject = stringDoubleDouble.Double;
      }
      else if (data is StockDataPoint)
      {
        StockDataPoint stockDataPoint = (StockDataPoint)data;
        xObject = stockDataPoint.DateTime;
        yObject = stockDataPoint.Low;
      }
      else if (data is BoxplotDataPoint)
      {
        BoxplotDataPoint box = (BoxplotDataPoint)data;
        if (ReverseAxes)
        {
          xObject = box.Minimum;
          yObject = box.Group;
        }
        else
        {
          xObject = box.Group;
          yObject = box.Minimum;
        }
      }
      else
      {
        xObject = null;
        yObject = null;
      }
    }

    #region Title property

    /// <summary>
    /// Gets or sets the title of the <see cref="DataSeries"/>.
    /// This is a dependency property.
    /// </summary>
    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register("Title", typeof(string), typeof(DataSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnTitleChanged)));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnTitleChanged();
    }

    internal virtual void OnTitleChanged()
    {
      if (_legendItem != null)
      {
        _legendItem.LegendLabel = Title;
      }
    }

    #endregion // Title property

    #region YAxisTitle property

    /// <summary>
    /// Gets or sets the title of the Y axis that this data series will be plotted against.
    /// This is used for plotting along an alternate Y axis of the <see cref="Chart"/>.
    /// This is a dependency property.
    /// </summary>
    public string YAxisTitle
    {
      get { return (string)GetValue(YAxisTitleProperty); }
      set { SetValue(YAxisTitleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YAxisTitle"/> property.
    /// </summary>
    public static readonly DependencyProperty YAxisTitleProperty =
      DependencyProperty.Register("YAxisTitle", typeof(string), typeof(DataSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnYAxisTitleChanged)));

    private static void OnYAxisTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnYAxisTitleChanged();
    }

    internal event EventHandler YAxisTitleChanged;

    private void OnYAxisTitleChanged()
    {
      EventHandler handler = YAxisTitleChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    #endregion // YAxisTitle property

    #region XAxisTitle Property

    /// <summary>
    /// Gets or sets the title of the X axis that this data series will be plotted against.
    /// This is used for plotting along an alternate X axis of the <see cref="Chart"/>.
    /// This is a dependency property.
    /// </summary>
    public string XAxisTitle
    {
      get { return (string)GetValue(XAxisTitleProperty); }
      set { SetValue(XAxisTitleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="XAxisTitle"/> property.
    /// </summary>
    public static readonly DependencyProperty XAxisTitleProperty =
      DependencyProperty.Register("XAxisTitle", typeof(string), typeof(DataSeries),
      new FrameworkPropertyMetadata(OnXAxisTitleChanged));

    private static void OnXAxisTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnXAxisTitleChanged();
    }

    internal event EventHandler XAxisTitleChanged;

    private void OnXAxisTitleChanged()
    {
      EventHandler handler = XAxisTitleChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    #endregion // XAxisTitle Property

    #region SelectionMode property

    /// <summary>
    /// Gets or sets the selection mode for the <see cref="DataPoint"/> objects displayed in the <see cref="DataSeries"/>.
    /// This is a dependency property.
    /// </summary>
    public DataPointSelectionMode SelectionMode
    {
      get { return (DataPointSelectionMode)GetValue(SelectionModeProperty); }
      set { SetValue(SelectionModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectionMode"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectionModeProperty =
      DependencyProperty.Register("SelectionMode", typeof(DataPointSelectionMode), typeof(DataSeries),
      new PropertyMetadata(DataPointSelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)));

    private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnSelectionModeChanged();
    }

    private void OnSelectionModeChanged()
    {
      if (SelectionMode == DataPointSelectionMode.Single && _selectedDataObjects.Count == 1)
      {
        return;
      }
      foreach (DataPoint dataPoint in _renderedDataPoints)
      {
        dataPoint.SetIsSelected(false);
      }
      _selectedDataObjects.Clear();
      SelectedDataPoint = null;
      SelectedItem = null;
    }

    #endregion // SelectionMode property

    #region HighlightMode property

    /// <summary>
    /// Gets or sets the highlight mode of the <see cref="DataPoint"/> objects in the <see cref="DataSeries"/>.
    /// This is a dependency property.
    /// </summary>
    public DataPointHighlightMode HighlightMode
    {
      get { return (DataPointHighlightMode)GetValue(HighlightModeProperty); }
      set { SetValue(HighlightModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HighlightMode"/> property.
    /// </summary>
    public static readonly DependencyProperty HighlightModeProperty =
      DependencyProperty.Register("HighlightMode", typeof(DataPointHighlightMode), typeof(DataSeries),
      new PropertyMetadata(DataPointHighlightMode.MouseOver, new PropertyChangedCallback(OnHighlightModeChanged)));

    private static void OnHighlightModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnHighlightModeChanged(e);
    }

    private void OnHighlightModeChanged(DependencyPropertyChangedEventArgs e)
    {
      DataPointHighlightMode oldMode = (DataPointHighlightMode)e.OldValue;
      if (oldMode == DataPointHighlightMode.None)
      {
        foreach (CartesianDataPoint point in _renderedDataPoints)
        {
          if (point != null)
          {
            point.CanHighlight = true;
          }
        }
      }
      else if (HighlightMode == DataPointHighlightMode.None)
      {
        foreach (CartesianDataPoint point in _renderedDataPoints)
        {
          if (point != null)
          {
            point.CanHighlight = false;
          }
        }
      }
    }

    private bool CanHighlight
    {
      get
      {
        return HighlightMode != DataPointHighlightMode.None;
      }
    }

    #endregion // HighlightMode property

    #region IsSelected Property

    /// <summary>
    /// Gets or sets whether or not this series is selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsSelectedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsSelected
    {
      get { return (bool)GetValue(IsSelectedProperty); }
      set { SetValue(IsSelectedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
      DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataSeries),
      new FrameworkPropertyMetadata(OnIsSelectedChanged));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnIsSelectedChanged();
    }

    private bool _selectionLock;

    private void OnIsSelectedChanged()
    {
      if (!_selectionLock)
      {
        if (SelectionMode == DataPointSelectionMode.Multiple || SelectionMode == DataPointSelectionMode.All)
        {
          _selectionLock = true;
          foreach (CartesianDataPoint dp in _renderedDataPoints)
          {
            if (dp != null)
            {
              dp.IsSelected = IsSelected;
            }
          }
          _selectionLock = false;
        }
      }

      EventHandler<EventArgs> handler = IsSelectedChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    #endregion // IsSelected Property

    internal event EventHandler<EventArgs> IsSelectedChanged;

    #region SeriesBrush property

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> used to color the <see cref="DataSeries"/>.
    /// This is a dependency property.
    /// </summary>
    public Brush SeriesBrush
    {
      get { return (Brush)GetValue(SeriesBrushProperty); }
      set { SetValue(SeriesBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SeriesBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty SeriesBrushProperty =
      DependencyProperty.Register("SeriesBrush", typeof(Brush), typeof(DataSeries),
      new PropertyMetadata(new SolidColorBrush(new Color() { A = 255, R = 40, G = 97, B = 169 }), new PropertyChangedCallback(OnSeriesBrushChanged)));

    private static void OnSeriesBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnSeriesBrushChanged(e);
    }

    private void OnSeriesBrushChanged(DependencyPropertyChangedEventArgs e)
    {
      foreach (DataPoint dp in _renderedDataPoints)
      {
        ChartSymbol symbol = dp as ChartSymbol;
        if (symbol != null)
        {
          if (symbol.Background == e.OldValue)
          {
            symbol.Background = SeriesBrush;
          }
          if (symbol.BorderBrush == e.OldValue)
          {
            symbol.BorderBrush = SeriesBrush;
          }
        }
      }
      // TODO: a faster way would be to iterate the collection of rendered data points. This may need a virtual method for specific series logic such as BarSeries.Brushes.
      RequestRebuild();
      OnLegendItemsChanged();
    }

    #endregion // SeriesBrush property

    #region IsShownInLegend Property

    /// <summary>
    /// Gets or sets whether or not this data series should be displayed in the legend.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsShownInLegendProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsShownInLegend
    {
      get { return (bool)GetValue(IsShownInLegendProperty); }
      set { SetValue(IsShownInLegendProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsShownInLegend"/> property.
    /// </summary>
    public static readonly DependencyProperty IsShownInLegendProperty =
      DependencyProperty.Register("IsShownInLegend", typeof(bool), typeof(DataSeries),
      new FrameworkPropertyMetadata(true, OnIsShownInLegendChanged));

    private static void OnIsShownInLegendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnIsShownInLegendChanged();
    }

    private void OnIsShownInLegendChanged()
    {
      OnLegendItemsChanged();
    }

    #endregion // IsShownInLegend Property

    /// <summary>
    /// Raised when the legend items for this <see cref="DataSeries"/> changes.
    /// </summary>
    public event EventHandler LegendItemsChanged;

    /// <summary>
    /// Raises the LegendItemsChanged event.
    /// </summary>
    protected void OnLegendItemsChanged()
    {
      EventHandler handler = LegendItemsChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    internal Canvas Canvas
    {
      get { return _canvas; }
      set
      {
        _canvas = value;
      }
    }

    internal Canvas ForegroundCanvas
    {
      get { return _foregroundCanvas; }
      set
      {
        _foregroundCanvas = value;
      }
    }

    internal Chart Chart
    {
      get { return _chart; }
      set
      {
        _chart = value;
      }
    }

    internal ObservableCollection<DataSeries> Series { get; set; }

    /// <summary>
    /// Gets the X axis that the <see cref="DataSeries"/> is plotted against.
    /// </summary>
    public ChartAxis XAxis
    {
      get { return _xAxis; }
      internal set
      {
        if (_xAxis != value)
        {
          _xAxis = value;
          OnXAxisChanged();
        }
      }
    }

    /// <summary>
    /// Called when the XAxis property changes.
    /// </summary>
    internal virtual void OnXAxisChanged()
    {
      // no-op in base class.
    }

    /// <summary>
    /// Gets the Y axis that the <see cref="DataSeries"/> is plotted against.
    /// </summary>
    public ChartAxis YAxis
    {
      get { return _yAxis; }
      internal set
      {
        if (_yAxis != value)
        {
          _yAxis = value;
          OnYAxisChanged();
        }
      }
    }

    /// <summary>
    /// Called when the YAxis property changes.
    /// </summary>
    internal virtual void OnYAxisChanged()
    {
      // no-op in base class.
    }

    /// <summary>
    /// Plots the <see cref="DataSeries"/> on the chart canvas.
    /// </summary>
    protected virtual void BuildChartCore()
    {
      PrepareToPlotData();

      if (ReverseAxes)
      {
        Plot_ReverseAxes();
      }
      else
      {
        Plot();
      }

      FinishPlottingData();
    }

    // This collection stores an ordered list of the indicies of the data that should be plotted.
    // This is used for always displaying min and max values from each sample space.
    private IList<int> _minMaxDataCache = null;

    private bool _isMinMaxCacheDirty = true;

    private void UpdateMinMaxDataCahche()
    {
      if (_isMinMaxCacheDirty && IndexStep > 1 && !ReverseAxes && IsMinMaxSamplingEnabled)
      {
        _minMaxDataCache = new List<int>();
        // Point usage: x = index, y = value
        Point minimum = new Point(0, Double.MaxValue);
        Point maximum = new Point(0, Double.MinValue);
        int count = 0;
        for (int i = 0; i < ItemsSource.Count; i++)
        {
          Point point = GetPoint(ItemsSource[i], i);
          if (point.Y < minimum.Y)
          {
            minimum.X = i;
            minimum.Y = point.Y;
          }
          if (point.Y >= maximum.Y)
          {
            maximum.X = i;
            maximum.Y = point.Y;
          }

          count++;
          if (count == IndexStep * 2)
          {
            _minMaxDataCache.Add((int)Math.Min(minimum.X, maximum.X));
            _minMaxDataCache.Add((int)Math.Max(minimum.X, maximum.X));
            minimum = new Point(0, Double.MaxValue);
            maximum = new Point(0, Double.MinValue);
            count = 0;
          }
        }
        _isMinMaxCacheDirty = false;
      }
      else if (IndexStep <= 1 || ReverseAxes || !IsMinMaxSamplingEnabled)
      {
        _minMaxDataCache = null;
      }
    }

    private void Plot()
    {
      UpdateMinMaxDataCahche();
      if (_minMaxDataCache != null)
      {
        PlotWithMinMaxCache();
        return;
      }

      int missingCacheIndex = 0;
      int missingIndex = _missingIndexCache.Count == 0 ? Int32.MaxValue : _missingIndexCache[missingCacheIndex];
      int index = MinimumIndex;
      if (index == missingIndex)
      {
        missingCacheIndex++;
        missingIndex = missingCacheIndex >= _missingIndexCache.Count ? Int32.MaxValue : _missingIndexCache[missingCacheIndex];
      }
      while (true)
      {
        if (index > MaximumIndex)
        {
          if (AlwaysPlotLastDataPoint)
          {
            index = MaximumIndex;
          }
          else
          {
            break;
          }
        }

        if (index >= 0 && index < ItemsSource.Count)
        {
          PlotDataPoint(index);
        }

        if (index == MaximumIndex)
        {
          break;
        }
        index += IndexStep;

        if (index == missingIndex)
        {
          missingCacheIndex++;
          missingIndex = missingCacheIndex >= _missingIndexCache.Count ? Int32.MaxValue : _missingIndexCache[missingCacheIndex];
        }
        else if (index > missingIndex)
        {
          index = missingIndex;
          missingCacheIndex++;
          missingIndex = missingCacheIndex >= _missingIndexCache.Count ? Int32.MaxValue : _missingIndexCache[missingCacheIndex];
        }
      }
    }

    private void PlotWithMinMaxCache()
    {
      int index = MinimumIndex;
      int count = 0;
      foreach (int i in _minMaxDataCache)
      {
        if (i >= index)
        {
          break;
        }
        count++;
      }
      int missingCacheIndex = 0;
      int missingIndex = _missingIndexCache.Count == 0 ? Int32.MaxValue : _missingIndexCache[missingCacheIndex];
      index = _minMaxDataCache[count];
      while (true)
      {
        if (index > MaximumIndex)
        {
          if (AlwaysPlotLastDataPoint)
          {
            index = MaximumIndex;
          }
          else
          {
            break;
          }
        }

        if (index >= 0 && index < ItemsSource.Count)
        {
          PlotDataPoint(index);
        }

        if (index == MaximumIndex)
        {
          break;
        }
        count++;
        if (count == _minMaxDataCache.Count)
        {
          break;
        }
        index = _minMaxDataCache[count];

        if (index == missingIndex)
        {
          missingCacheIndex++;
          missingIndex = missingCacheIndex >= _missingIndexCache.Count ? Int32.MaxValue : _missingIndexCache[missingCacheIndex];
        }
        else if (index > missingIndex)
        {
          index = missingIndex;
          missingCacheIndex++;
          missingIndex = missingCacheIndex >= _missingIndexCache.Count ? Int32.MaxValue : _missingIndexCache[missingCacheIndex];
        }
      }
    }

    private void Plot_ReverseAxes()
    {
      int index = MinimumIndex;
      while (true)
      {
        if (index > MaximumIndex)
        {
          if (AlwaysPlotLastDataPoint)
          {
            index = MaximumIndex;
          }
          else
          {
            break;
          }
        }

        if (index >= 0 && index < ItemsSource.Count)
        {
          PlotDataPoint_ReverseAxes(index);
        }

        if (index == MaximumIndex)
        {
          break;
        }
        else
        {
          index += IndexStep;
        }
      }
    }

    internal virtual void PrepareToPlotData() { }

    internal virtual void PlotDataPoint(int index) { }

    internal virtual void PlotDataPoint_ReverseAxes(int index) { }

    internal virtual void FinishPlottingData() { }

    /// <summary>
    /// Gets whether the <see cref="DataSeries"/> supports sampling for large data sets.
    /// </summary>
    protected virtual bool SupportsDataSampling
    {
      get { return false; }
    }

    #region AllowsDataSampling Property

    /// <summary>
    /// Gets or sets whether or not this <see cref="DataSeries"/> will use data sampling. Data sampling is used to skip over a calculated number of
    /// data points to improve the performance of plotting charts with large data sets. The default is true.
    /// Note that it is possible for data sampling to skip over outlying data points.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowsDataSamplingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowsDataSampling
    {
      get { return (bool)GetValue(AllowsDataSamplingProperty); }
      set { SetValue(AllowsDataSamplingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowsDataSampling"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowsDataSamplingProperty =
      DependencyProperty.Register("AllowsDataSampling", typeof(bool), typeof(DataSeries),
      new FrameworkPropertyMetadata(true, OnAllowsDataSamplingChanged));

    private static void OnAllowsDataSamplingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnAllowsDataSamplingChanged();
    }

    private void OnAllowsDataSamplingChanged()
    {
    }

    #endregion // AllowsDataSampling Property

    #region IsDataSamplingUsed Property

    /// <summary>
    /// Gets whether or not this <see cref="DataSeries"/> is currently skipping some data points.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsDataSamplingUsedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsDataSamplingUsed
    {
      get { return (bool)GetValue(IsDataSamplingUsedProperty); }
    }

    private static readonly DependencyPropertyKey IsDataSamplingUsedPropertyKey =
        DependencyProperty.RegisterReadOnly("IsDataSamplingUsed", typeof(bool), typeof(DataSeries), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsDataSamplingUsed"/> property.
    /// </summary>
    public static readonly DependencyProperty IsDataSamplingUsedProperty =
        IsDataSamplingUsedPropertyKey.DependencyProperty;

    #endregion // IsDataSamplingUsed Property

    #region IsMinMaxSamplingEnabled Property

    /// <summary>
    /// Gets or sets the IsMinMaxSamplingEnabled.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsMinMaxSamplingEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsMinMaxSamplingEnabled
    {
      get { return (bool)GetValue(IsMinMaxSamplingEnabledProperty); }
      set { SetValue(IsMinMaxSamplingEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsMinMaxSamplingEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsMinMaxSamplingEnabledProperty =
      DependencyProperty.Register("IsMinMaxSamplingEnabled", typeof(bool), typeof(DataSeries),
      new FrameworkPropertyMetadata(OnIsMinMaxSamplingEnabledChanged));

    private static void OnIsMinMaxSamplingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnIsMinMaxSamplingEnabledChanged();
    }

    private void OnIsMinMaxSamplingEnabledChanged()
    {
      _isMinMaxCacheDirty = true;
      RequestRebuild();
    }

    #endregion // IsMinMaxSamplingEnabled Property

    internal void UpdateOptimizationProperties()
    {
      if (ItemsSource != null && ItemsSource.Count > 0 && Visibility == Visibility.Visible)
      {
        if (SupportsDataSampling && IsDataOrdered)
        {
          SetOptimisationProperties();
          if (!AllowsDataSampling)
          {
            IndexStep = 1;
          }
        }
        else
        {
          IndexStep = 1;
          MinimumIndex = 0;
          MaximumIndex = ItemsSource.Count - 1;
        }
      }
    }

    internal virtual void BuildChart()
    {
      if (ItemsSource != null && ItemsSource.Count > 0 && Visibility == Visibility.Visible)
      {
        _recycleIndex = 0;
        if (XAxis != null && XAxis.ConversionRatio > 0 && YAxis != null && YAxis.ConversionRatio > 0)
        {
          BuildChartCore();
        }
        while (_renderedDataPoints.Count > _recycleIndex)
        {
          _renderedDataPoints.RemoveAt(_renderedDataPoints.Count - 1);
        }
      }
    }

    private void SetOptimisationProperties()
    {
      int indexStep = IndexStep;
      if (ItemsSource != null && ItemsSource.Count > 0 && XAxis != null && YAxis != null)
      {
        // Calculate minimum and maximum data indicies.
        MinimumIndex = CalculateMinimumIndex(0, ItemsSource.Count - 1, ReverseAxes ? YAxis : XAxis);
        MaximumIndex = CalculateMaximumIndex(0, ItemsSource.Count - 1, ReverseAxes ? YAxis : XAxis);
        // Calculate index step.
        int oldIndexStep = IndexStep;
        IndexStep = 1;
        if (DataSampler != null)
        {
          IndexStep = DataSampler.CalculateIndexStep(MaximumIndex - MinimumIndex, new Size(_canvas.ActualWidth, _canvas.ActualHeight));
        }
        else if (MaximumIndex - MinimumIndex > 400)
        {
          IndexStep = Math.Max(1, (MaximumIndex - MinimumIndex) / 400);
          IndexStep = AdjustIndexStep(IndexStep);
        }
        //Adjust minimum and maximum data indicies.
        MinimumIndex = Math.Max(0, MinimumIndex - (MinimumIndex % IndexStep));
        MaximumIndex = Math.Min(ItemsSource.Count - 1, MaximumIndex - (MaximumIndex % IndexStep) + IndexStep);

        if (IndexStep != oldIndexStep)
        {
          if (XAxis != null)
          {
            XAxis.UpdateSpacing();
            XAxis.UpdateAxisLater(); // TODO: it would be nice to remove this dependancy
          }
          if (YAxis != null)
          {
            YAxis.UpdateSpacing();
            YAxis.UpdateAxisLater(); // TODO: it would be nice to remove this dependancy
          }
        }
      }
      if (indexStep != IndexStep)
      {
        _isMinMaxCacheDirty = true;
      }
    }

    #region DataSampler Property

    /// <summary>
    /// Gets or sets the <see cref="IChartDataSampler"/> used for calculating the index interval.
    /// The default is a <see cref="FixedSampleCountSampler"/> with a max data point count of 400.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DataSamplerProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IChartDataSampler DataSampler
    {
      get { return (IChartDataSampler)GetValue(DataSamplerProperty); }
      set { SetValue(DataSamplerProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DataSampler"/> property.
    /// </summary>
    public static readonly DependencyProperty DataSamplerProperty =
      DependencyProperty.Register("DataSampler", typeof(IChartDataSampler), typeof(DataSeries),
      new FrameworkPropertyMetadata(new FixedSampleCountSampler() { MaxDataPointCount = 400 }, OnDataSamplerChanged));

    private static void OnDataSamplerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnDataSamplerChanged();
    }

    private void OnDataSamplerChanged()
    {
      RequestRebuild();
    }

    #endregion // DataSampler Property

    private int AdjustIndexStep(int indexStep)
    {
      double step = indexStep;
      int count = 0;
      while (step > 1)
      {
        step /= 2.0;
        count++;
      }
      return (int)Math.Pow(2, count);
    }

    private int CalculateMinimumIndex(int startIndex, int endIndex, ChartAxis independantAxis)
    {
      if (endIndex - startIndex < 2)
      {
        return startIndex;
      }
      int middleIndex = (int)Math.Round((startIndex + endIndex) / 2.0);
      double data = GetData(middleIndex);
      if (data.Equals(Double.NaN))
      {
        return 0;
      }
      if (data >= independantAxis.ActualMinimumValue)
      {
        return CalculateMinimumIndex(startIndex, middleIndex, independantAxis);
      }
      return CalculateMinimumIndex(middleIndex, endIndex, independantAxis);
    }

    private int CalculateMaximumIndex(int startIndex, int endIndex, ChartAxis independantAxis)
    {
      if (endIndex - startIndex < 2)
      {
        return endIndex;
      }
      int middleIndex = (int)Math.Round((startIndex + endIndex) / 2.0);
      double data = GetData(middleIndex);
      if (data.Equals(Double.NaN))
      {
        return ItemsSource.Count - 1;
      }
      if (data <= independantAxis.ActualMaximumValue)
      {
        return CalculateMaximumIndex(middleIndex, endIndex, independantAxis);
      }
      return CalculateMaximumIndex(startIndex, middleIndex, independantAxis);
    }

    private double GetData(int index)
    {
      Point point = GetPoint(ItemsSource[index], index);
      return ReverseAxes ? point.Y : point.X;
    }

    /// <summary>
    /// Gets the index of the data that is closest to the actual minimum value of the independent axis.
    /// Custom data series can use this value to optimise chart rendering.
    /// </summary>
    /// <remarks>This property should be accessed only from the <see cref="BuildChartCore"/> method,
    /// and is only meaningful if <see cref="SupportsDataSampling"/> is true for the series.</remarks>
    protected int MinimumIndex { get; private set; }

    /// <summary>
    /// Gets the index of the data that is closest to the actual maximum value of the independent axis.
    /// Custom data series can use this value to optimise chart rendering.
    /// </summary>
    /// <remarks>This property should be accessed only from the <see cref="BuildChartCore"/> method,
    /// and is only meaningful if <see cref="SupportsDataSampling"/> is true for the series.</remarks>
    protected int MaximumIndex { get; private set; }

    internal bool IsDataOrdered { get; set; }

    private int _indexStep;

    /// <summary>
    /// Gets the index step between data point that are to be rendered.
    /// Custom data series can use this value to optimise chart rendering.
    /// </summary>
    /// <remarks>This property should be accessed only from the <see cref="BuildChartCore"/> method,
    /// and is only meaningful if <see cref="SupportsDataSampling"/> is true for the series.</remarks>
    protected int IndexStep
    {
      get { return _indexStep; }
      private set
      {
        if (_indexStep != value)
        {
          _indexStep = value;
          SetValue(IsDataSamplingUsedPropertyKey, _indexStep > 1);
        }
      }
    }

    internal int GetIndexStep()
    {
      return IndexStep;
    }

    /// <summary>
    /// Converts the given logical data point to a physical point on the chart canvas.
    /// </summary>
    /// <param name="point">The logical data point to convert.</param>
    /// <returns>A physical position on the chart canvas based on the given logical data point.</returns>
    public Point ConvertLogicalToPhysicalPoint(Point point)
    {
      if (XAxis == null || YAxis == null)
      {
        return new Point();
      }
      double x = XAxis.ConvertLogicalToPhysical(point.X);
      double y = Canvas.ActualHeight - YAxis.ConvertLogicalToPhysical(point.Y);

      return new Point(Double.IsInfinity(x) ? 0 : x, Double.IsInfinity(y) ? 0 : y);
    }

    /// <summary>
    /// Converts the given physical position to a logical point based on the X and Y axes that this series uses.
    /// </summary>
    /// <param name="point">The physical position to convert.</param>
    /// <returns>A logical point based on the given physical position.</returns>
    public Point ConvertPhysicalToLogicalPoint(Point point)
    {
      if (XAxis == null || YAxis == null)
      {
        return new Point();
      }
      double x = XAxis.ConvertPhysicalToLogical(point.X);
      double y = YAxis.ConvertPhysicalToLogical(point.Y);

      return new Point(Double.IsInfinity(x) ? 0 : x, Double.IsInfinity(y) ? 0 : y);
    }

    private Binding _xBinding;
    private Binding _yBinding;

    private PropertyInfo _xPropertyInfo;
    private PropertyInfo _yPropertyInfo;

    private void UpdatePropertyInfo()
    {
      _xPropertyInfo = null;
      _yPropertyInfo = null;
      if (ItemsSource != null && ItemsSource.Count > 0)
      {
        object o = ItemsSource[0];
        UpdatePropertyInfo(o);
      }
    }

    private void UpdatePropertyInfo(object o)
    {
      _xPropertyInfo = null;
      _yPropertyInfo = null;
      if (o != null)
      {
        if (XBinding != null)
        {
          _xPropertyInfo = GetProperty(o, XBinding);
        }
        if (YBinding != null)
        {
          _yPropertyInfo = GetProperty(o, YBinding);
        }
      }
    }

    private PropertyInfo GetProperty(object o, Binding binding)
    {
      string path = binding.Path.Path;
      if (string.IsNullOrEmpty(binding.Path.Path) || path == "(0)")
      {
        if (binding.Path.PathParameters.Count == 0)
        {
          // binds to source object
          return new SelfPropertyInfo(binding);
        }
        if (binding.Path.PathParameters.Count == 1)
        {
          object parameter = binding.Path.PathParameters[0];
          if (parameter is PropertyDescriptor)
          {
            return new PropertyDescriptorInfo((PropertyDescriptor)parameter, binding);
          }
          if (parameter is DependencyProperty)
          {
            return new DependencyPropertyInfo((DependencyProperty)parameter, binding);
          }
        }
        // any other cases?
        return new BindingPropertyInfo(binding);
      }
      else if (path.Contains(".") || path.Contains("(") || path.Contains("["))
      {
        // a complex path
        return new BindingPropertyInfo(binding);
      }
      else if (o is ICustomTypeDescriptor)
      {
        PropertyDescriptor descriptor = ((ICustomTypeDescriptor)o).GetProperties()[path];
        if (descriptor != null)
        {
          return new PropertyDescriptorInfo(descriptor, binding);
        }
        return new BindingPropertyInfo(binding);
      }
      else if (binding.Converter != null)
      {
        return new PropertyInfoWithConverter(o.GetType().GetProperty(path), binding);
      }
      else
      {
        // just the name of a property, without converter
        return o.GetType().GetProperty(path);
      }
    }

    /// <summary>
    /// Gets or sets a binding used to extract the X axis value from a data object.
    /// </summary>
    public Binding XBinding
    {
      get { return _xBinding; }
      set
      {
        _xBinding = value;
        _xExtractor.SetBinding(ValueExtractor.ValueProperty, _xBinding);
        UpdatePropertyInfo();
      }
    }

    /// <summary>
    /// Gets or sets a binding used to extract the Y axis value from a data object.
    /// </summary>
    public Binding YBinding
    {
      get { return _yBinding; }
      set
      {
        _yBinding = value;
        _yExtractor.SetBinding(ValueExtractor.ValueProperty, _yBinding);
        UpdatePropertyInfo();
      }
    }

    private IChartDataExtractor _dataExtractor;

    /// <summary>
    /// Gets or sets the data extractor used to get X and Y values from custom data point model objects. This is a high performance alternative to using XBinding and YBinding.
    /// </summary>
    public IChartDataExtractor DataExtractor
    {
      get { return _dataExtractor; }
      set
      {
        _dataExtractor = value;
      }
    }

    /// <summary>
    /// Gets or sets a binding used to extract the label content from a data point model object.
    /// </summary>
    public Binding LabelBinding { get; set; }

    /// <summary>
    /// Gets or sets a binding used to extract the tool tip content from a data point model object.
    /// </summary>
    public Binding ToolTipBinding { get; set; }

    // TODO: this shouldn't really be public should it?
    /// <summary>
    /// Gets a read only collection containing the <see cref="CartesianDataPoint"/> objects within the <see cref="DataSeries"/>.
    /// A <see cref="CartesianDataPoint"/> is the graphical representation of plotted data and is created by the <see cref="DataSeries"/>.
    /// </summary>
    public ReadOnlyCollection<CartesianDataPoint> DataPoints
    {
      get
      {
        return new ReadOnlyCollection<CartesianDataPoint>(_renderedDataPoints);
      }
    }

    private readonly IList<CartesianDataPoint> _renderedDataPoints = new List<CartesianDataPoint>();
    private int _recycleIndex = 0;

    private CartesianDataPoint _highlightedDataPoint;

    internal void HighlightClosestDataPoint(double logicalX)
    {
      if (HighlightMode == DataPointHighlightMode.ClosestXValue)
      {
        if (_highlightedDataPoint != null)
        {
          _highlightedDataPoint.IsHighlighted = false;
        }
        double diff = Double.MaxValue;
        //TODO: use a binary search here to improve performance:
        foreach (CartesianDataPoint dataPoint in _renderedDataPoints)
        {
          double xValue = dataPoint.LogicalPoint.X;
          double currentDiff = Math.Abs(logicalX - xValue);
          if (currentDiff > diff)
          {
            break;
          }
          diff = currentDiff;
          _highlightedDataPoint = dataPoint;
        }
        if (_highlightedDataPoint != null)
        {
          _highlightedDataPoint.IsHighlighted = true;
        }
      }
    }

    internal void UnHighlightDataPoints()
    {
      if (_highlightedDataPoint != null)
      {
        _highlightedDataPoint.IsHighlighted = false;
      }
    }

    /// <summary>
    /// Gets the <see cref="CartesianDataPoint"/> mapped to the given data object and index.
    /// </summary>
    /// <param name="o">The data object.</param>
    /// <returns>The <see cref="CartesianDataPoint"/> mapped to the given object if it exists.</returns>
    /// <param name="index">The index of the data object within the ItemsSource.</param>
    internal CartesianDataPoint GetDataPoint(object o, int index)
    {
      int recycleIndex = _recycleIndex;
      if (index == -1)
      {
        recycleIndex--; // This is a kludge for the poorly implemented AddDataLabel methods. TODO: resolve this
      }
      CartesianDataPoint dataPoint = null;
      if (recycleIndex < _renderedDataPoints.Count && recycleIndex >= 0)
      {
        dataPoint = _renderedDataPoints[recycleIndex];
        dataPoint.XObjectChanged -= new EventHandler(DataPoint_XObjectChanged);
        dataPoint.YObjectChanged -= new EventHandler(DataPoint_YObjectChanged);
        dataPoint.IsSelectedChangeRequested -= new EventHandler<DataPoint.SelectionChangeRequestArgs>(DataPoint_IsSelectedChangeRequested);
        dataPoint.IsSelected = IsObjectSelected(o);
        dataPoint.DataContext = o;
        GetPoint(dataPoint, index);
      }
      return dataPoint;
    }

    private bool IsObjectSelected(object o)
    {
      if (IsSelected && (SelectionMode == DataPointSelectionMode.Multiple || SelectionMode == DataPointSelectionMode.All))
      {
        return true;
      }
      return _selectedDataObjects.Contains(o);
    }

    #region SelectedDataPoint Property

    /// <summary>
    /// Gets the currently selected <see cref="CartesianDataPoint"/>.
    /// This will be null if none of the data points in the <see cref="DataSeries"/> are selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedDataPointProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public CartesianDataPoint SelectedDataPoint
    {
      get { return (CartesianDataPoint)GetValue(SelectedDataPointProperty); }
      private set
      {
        SetValue(SelectedDataPointPropertyKey, value);
      }
    }

    private static readonly DependencyPropertyKey SelectedDataPointPropertyKey =
        DependencyProperty.RegisterReadOnly("SelectedDataPoint", typeof(CartesianDataPoint), typeof(DataSeries),
        new UIPropertyMetadata(null, OnSelectedDataPointChanged));

    /// <summary>
    /// Identifies the <see cref="SelectedDataPoint"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedDataPointProperty =
        SelectedDataPointPropertyKey.DependencyProperty;

    private static void OnSelectedDataPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnSelectedDataPointChanged();
    }

    private void OnSelectedDataPointChanged()
    {
      if (!_selectedItemLock)
      {
        _selectedItemLock = true;
        if (SelectedDataPoint != null)
        {
          SelectedItem = SelectedDataPoint.DataContext;
        }
        else
        {
          SelectedDataPoint = null;
        }
        _selectedItemLock = false;
      }

      EventHandler<SelectedDataPointChangedEventArgs> handler = SelectedDataPointChanged;
      if (handler != null)
      {
        handler(this, new SelectedDataPointChangedEventArgs());
      }
    }

    /// <summary>
    /// Raised when the selected data point changes.
    /// </summary>
    public event EventHandler<SelectedDataPointChangedEventArgs> SelectedDataPointChanged;

    #endregion // SelectedDataPoint Property

    #region SelectedItem Property

    /// <summary>
    /// Gets or sets the selected data object.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedItemProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object SelectedItem
    {
      get { return GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty =
      DependencyProperty.Register("SelectedItem", typeof(object), typeof(DataSeries),
      new FrameworkPropertyMetadata(OnSelectedItemChanged));

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataSeries)d).OnSelectedItemChanged();
    }

    private bool _selectedItemLock;

    private void OnSelectedItemChanged()
    {
      if (!_selectedItemLock)
      {
        _selectedItemLock = true;
        _selectedDataObjects.Clear();
        if (SelectedItem != null)
        {
          _selectedDataObjects.Add(SelectedItem);
        }
        RequestRebuild(); // A bit slow if thrashing this property, but good enough for now to handle any selection scenarios.
        _selectedItemLock = false;
      }
    }

    #endregion // SelectedItem Property

    /*private readonly ObservableCollection<CartesianDataPoint> _selectedDataPoints = new ObservableCollection<CartesianDataPoint>();

    private ObservableCollection<CartesianDataPoint> SelectedDataPoints
    {
      get { return _selectedDataPoints; }
    }*/

    private readonly HashSet<object> _selectedDataObjects = new HashSet<object>();

    /// <summary>
    /// Prepares the given <see cref="CartesianDataPoint"/> to participate in selection features and data notifications.
    /// </summary>
    /// <param name="dataPoint">The <see cref="CartesianDataPoint"/> to prepare.</param>
    /// <param name="index">The index of the data object within the ItemsSource.</param>
    protected void PrepareDataPoint(CartesianDataPoint dataPoint, int index)
    {
      if (dataPoint != null)
      {
        AttachSelectionHandler(dataPoint);
        AttachDataListeners(dataPoint);
        if (_recycleIndex < _renderedDataPoints.Count)
        {
          _renderedDataPoints[_recycleIndex] = dataPoint;
        }
        else
        {
          _renderedDataPoints.Add(dataPoint);
        }
        _recycleIndex++;
        dataPoint.CanHighlight = CanHighlight;
        SetLabelBinding(dataPoint);
        SetToolTipBinding(dataPoint);
        dataPoint.DataSeries = this;
      }
    }

    private void SetLabelBinding(DataPoint dataPoint)
    {
      if (LabelBinding != null)
      {
        dataPoint.SetBinding(DataPoint.LabelContentProperty, LabelBinding);
      }
    }

    private void SetToolTipBinding(DataPoint dataPoint)
    {
      if (ToolTipBinding != null)
      {
        dataPoint.SetBinding(DataPoint.ToolTipProperty, ToolTipBinding);
      }
    }

    private void AttachDataListeners(CartesianDataPoint dataPoint)
    {
      dataPoint.XObjectChanged += new EventHandler(DataPoint_XObjectChanged);
      dataPoint.YObjectChanged += new EventHandler(DataPoint_YObjectChanged);
    }

    private void DataPoint_YObjectChanged(object sender, EventArgs e)
    {
      RequestAnalyseAndRebuildAll();
    }

    private void DataPoint_XObjectChanged(object sender, EventArgs e)
    {
      RequestAnalyseAndRebuildAll();
    }

    private void AttachSelectionHandler(CartesianDataPoint dataPoint)
    {
      // Removing handler to ensure we don't stack up.
      //dataPoint.IsSelectedChangeRequested -= new EventHandler<DataPoint.SelectionChangeRequestArgs>(DataPoint_IsSelectedChangeRequested);
      dataPoint.IsSelectedChangeRequested += new EventHandler<DataPoint.SelectionChangeRequestArgs>(DataPoint_IsSelectedChangeRequested);
    }

    private void DataPoint_IsSelectedChangeRequested(object sender, DataPoint.SelectionChangeRequestArgs e)
    {
      if (Chart == null)
      {
        e.ActualSelection = !e.RequestedSelection;
        return;
      }
      if (Mouse.RightButton == MouseButtonState.Pressed && !Chart.IsRightClickSelectionEnabled)
      {
        e.ActualSelection = !e.RequestedSelection;
        return;
      }
      if (!Chart.CanToggleSelection && e.RequestedSelection == false)
      {
        e.ActualSelection = true;
        return;
      }
      CartesianDataPoint dataPoint = sender as CartesianDataPoint;
      if (dataPoint == null)
      {
        e.ActualSelection = false;
        return;
      }
      switch (SelectionMode)
      {
        case DataPointSelectionMode.Single:
          if (SelectedDataPoint != null)
          {
            SelectedDataPoint.SetIsSelected(false);
          }
          foreach (CartesianDataPoint dp in _renderedDataPoints)
          {
            if (dp != null && dp != dataPoint)
            {
              dp.SetIsSelected(false);
            }
          }
          SelectedDataPoint = dataPoint.IsSelected ? dataPoint : null;
          _selectedDataObjects.Clear();
          if (dataPoint.IsSelected)
          {
            AddSelectedDataPoint(dataPoint);
          }
          else
          {
            RemoveSelectedDataPoint(dataPoint);
          }
          break;
        case DataPointSelectionMode.All:
          foreach (CartesianDataPoint dp in _renderedDataPoints)
          {
            if (dp != null)
            {
              dp.SetIsSelected(e.RequestedSelection);
            }
          }
          SelectedDataPoint = dataPoint.IsSelected ? dataPoint : null;
          IsSelected = dataPoint.IsSelected;
          break;
        case DataPointSelectionMode.Multiple:
          if (dataPoint.IsSelected)
          {
            AddSelectedDataPoint(dataPoint);
            SelectedDataPoint = dataPoint;
          }
          else
          {
            RemoveSelectedDataPoint(dataPoint);
            if (SelectedDataPoint != null && SelectedDataPoint.DataContext == dataPoint.DataContext)
            {
              bool foundSelectedPoint = false;
              foreach (CartesianDataPoint dp in _renderedDataPoints)
              {
                if (dp != null && dp.IsSelected)
                {
                  SelectedDataPoint = dp;
                  foundSelectedPoint = true;
                  break;
                }
              }
              if (!foundSelectedPoint)
              {
                SelectedDataPoint = null;
              }
            }
          }
          if (!_selectionLock)
          {
            bool isSelected = true;
            foreach (CartesianDataPoint dp in _renderedDataPoints)
            {
              if (dp != null && !dp.IsSelected)
              {
                isSelected = false;
                break;
              }
            }
            _selectionLock = true;
            IsSelected = isSelected;
            _selectionLock = false;
          }
          break;
        case DataPointSelectionMode.None:
          e.ActualSelection = false;
          IsSelected = false;
          break;
      }
    }

    private void AddSelectedDataPoint(CartesianDataPoint dp)
    {
      if (dp != null && dp.DataContext != null)
      {
        _selectedDataObjects.Add(dp.DataContext);
      }
    }

    private void RemoveSelectedDataPoint(CartesianDataPoint dp)
    {
      if (dp != null && dp.DataContext != null)
      {
        _selectedDataObjects.Remove(dp.DataContext);
      }
    }

    internal void DeselectAll()
    {
      foreach (DataPoint dataPoint in _renderedDataPoints) // TODO: should be a way to only iterate the selected items
      {
        if (dataPoint != null)
        {
          dataPoint.SetIsSelected(false);
        }
      }
      _selectedDataObjects.Clear();
      SelectedDataPoint = null;
    }

    /// <summary>
    /// Returns a <see cref="Point"/> containing the X and Y logical axis values of the given <see cref="DataPoint"/>.
    /// In the case that the given data object is a primitive numerical value, the reverseAxes parameter
    /// can be used to specify that YAxis is the independent axis instead of the XAxis by default. This is useful for
    /// data series such as horizontal bar charts.
    /// </summary>
    /// <param name="dataPoint">The <see cref="CartesianDataPoint"/> holding the data to calculate a <see cref="Point"/> for.</param>
    /// <param name="index">The index of the data point in the items source</param>
    /// <returns>A <see cref="Point"/> containg the X and Y axis values for the given data object.</returns>
    internal Point GetPoint(CartesianDataPoint dataPoint, int index)
    {
      object xObject, yObject;
      Point point = GetPoint(dataPoint.DataContext, index, out xObject, out yObject);
      dataPoint.LogicalPoint = point;
      //if (XBinding != null)
      {
        // TODO: these bindings are needed for property change notification. But the GetPoint method already sets up bindings which makes this slower.
        // Find a way to reduce the number of binding operations.
        //dataPoint.SetBinding(CartesianDataPoint.XObjectProperty, XBinding);
      }
      
      //else
      {
        dataPoint.XObject = xObject;// XAxis != null ? GetObject(XAxis.GetLabel(point.X)) : point.X; // TODO: is this GetLabel thing doing anything apart from dropping the performance??
      }
      //if (YBinding != null)
      {
        //dataPoint.SetBinding(CartesianDataPoint.YObjectProperty, YBinding);
      }
      //else
      {
        dataPoint.YObject = yObject;// YAxis != null ? GetObject(YAxis.GetLabel(point.Y)) : point.Y;
      }
      return point;
    }

    private object GetObject(string label)
    {
      double d;
      bool success = Double.TryParse(label, out d);
      if (success)
      {
        return d;
      }
      return label;
    }
  }

  internal class ValueExtractor : Control
  {
    #region Value Property

    public object Value
    {
      get { return GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register("Value", typeof(object), typeof(ValueExtractor), null);

    #endregion // Value Property
  }

  internal abstract class PropertyInfoBase : PropertyInfo
  {
    private readonly Binding _binding;

    public PropertyInfoBase(Binding binding)
    {
      _binding = binding;
    }

    public new abstract object GetValue(object component);

    public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
    {
      object value = GetValue(obj);
      if (_binding.Converter != null)
      {
        value = _binding.Converter.Convert(value, typeof(object), _binding.ConverterParameter, _binding.ConverterCulture);
      }
      return value;
    }

    // none of these is ever called, so no need to implement.
    public override PropertyAttributes Attributes { get { throw new NotImplementedException(); } }
    public override bool CanRead { get { throw new NotImplementedException(); } }
    public override bool CanWrite { get { throw new NotImplementedException(); } }
    public override MethodInfo[] GetAccessors(bool nonPublic) { throw new NotImplementedException(); }
    public override MethodInfo GetGetMethod(bool nonPublic) { throw new NotImplementedException(); }
    public override ParameterInfo[] GetIndexParameters() { throw new NotImplementedException(); }
    public override MethodInfo GetSetMethod(bool nonPublic) { throw new NotImplementedException(); }
    public override Type PropertyType { get { throw new NotImplementedException(); } }
    public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture) { throw new NotImplementedException(); }
    public override Type DeclaringType { get { throw new NotImplementedException(); } }
    public override object[] GetCustomAttributes(Type attributeType, bool inherit) { throw new NotImplementedException(); }
    public override object[] GetCustomAttributes(bool inherit) { throw new NotImplementedException(); }
    public override bool IsDefined(Type attributeType, bool inherit) { throw new NotImplementedException(); }
    public override string Name { get { throw new NotImplementedException(); } }
    public override Type ReflectedType { get { throw new NotImplementedException(); } }
  }

  internal class SelfPropertyInfo : PropertyInfoBase
  {
    public SelfPropertyInfo(Binding binding)
      : base(binding)
    {
    }

    public override object GetValue(object obj)
    {
      return obj;
    }
  }

  internal class PropertyInfoWithConverter : PropertyInfoBase
  {
    private readonly PropertyInfo _PropertyInfo;

    public PropertyInfoWithConverter(PropertyInfo propertyInfo, Binding binding)
      : base(binding)
    {
      _PropertyInfo = propertyInfo;
    }

    public override object GetValue(object obj)
    {
      return _PropertyInfo.GetValue(obj, null);
    }
  }

  internal class PropertyDescriptorInfo : PropertyInfoBase
  {
    private readonly PropertyDescriptor _PropertyDescriptor;

    public PropertyDescriptorInfo(PropertyDescriptor propertyDescriptor, Binding binding)
      : base(binding)
    {
      _PropertyDescriptor = propertyDescriptor;
    }

    public override object GetValue(object obj)
    {
      return _PropertyDescriptor.GetValue(obj);
    }
  }

  internal class DependencyPropertyInfo : PropertyInfoBase
  {
    private readonly DependencyProperty _dependencyProperty;

    public DependencyPropertyInfo(DependencyProperty dp, Binding binding)
      : base(binding)
    {
      _dependencyProperty = dp;
    }

    public override object GetValue(object obj)
    {
      return ((DependencyObject)obj).GetValue(_dependencyProperty);
    }
  }

  internal class BindingPropertyInfo : PropertyInfoBase
  {
    private readonly ValueExtractor _extractor;

    public BindingPropertyInfo(Binding binding)
      : base(binding)
    {
      _extractor = new ValueExtractor() { DataContext = null };
      _extractor.SetBinding(ValueExtractor.ValueProperty, binding);
    }

    // Not used in this implementation.
    public override object GetValue(object obj)
    {
      return obj;
    }

    public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
    {
      _extractor.DataContext = obj;
      return _extractor.Value;
    }
  }
}

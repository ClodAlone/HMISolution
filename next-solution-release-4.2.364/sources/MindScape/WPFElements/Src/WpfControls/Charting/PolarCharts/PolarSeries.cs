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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a data series that can be plotted by a <see cref="PolarChart"/> control.
  /// </summary>
  public abstract class PolarSeries : Control, INotifyPropertyChanged
  {
    private Canvas _canvas;
    private ThetaAxis _thetaAxis;
    private RhoAxis _rhoAxis;

    /// <summary>
    /// Gets a list of legend items to be displayed in the legend for this <see cref="PolarSeries"/>.
    /// </summary>
    public virtual IList<LegendItem> LegendItems
    {
      get
      {
        IList<LegendItem> items = new List<LegendItem>();
        LegendItem item = new LegendItem(Title, LegendIconTemplate, SeriesBrush, this);
        items.Add(item);
        return items;
      }
    }

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

    internal ObservableCollection<PolarSeries> Series { get; set; }

    /// <summary>
    /// Gets the theta axis that the <see cref="PolarSeries"/> is plotted against.
    /// </summary>
    internal ThetaAxis ThetaAxis
    {
      get { return _thetaAxis; }
      set
      {
        if (_thetaAxis != value)
        {
          _thetaAxis = value;
          if (ThetaAxis != null && RhoAxis != null && ThetaAxis.Minimum == ThetaAxis.Maximum && ItemsSource != null && ItemsSource.Count > 0)
          {
            AnalyseSeries();
          }
          //OnThetaAxisChanged();
        }
      }
    }

    /*
    /// <summary>
    /// Called when the ThetaAxis property changes.
    /// </summary>
    internal virtual void OnThetaAxisChanged()
    {
      // no-op in base class.
    }
    */

    /// <summary>
    /// Gets the rho axis that the <see cref="PolarSeries"/> is plotted against.
    /// </summary>
    internal RhoAxis RhoAxis
    {
      get { return _rhoAxis; }
      set
      {
        if (_rhoAxis != value)
        {
          _rhoAxis = value;
          if (RhoAxis != null && ThetaAxis != null && RhoAxis.Minimum == RhoAxis.Maximum && ItemsSource != null && ItemsSource.Count > 0)
          {
            AnalyseSeries();
          }
          //OnRhoAxisChanged();
        }
      }
    }

    /*
    /// <summary>
    /// Called when the RhoAxis property changes.
    /// </summary>
    internal virtual void OnRhoAxisChanged()
    {
      // no-op in base class.
    }
    */

    /// <summary>
    /// Plots the <see cref="DataSeries"/> on the chart canvas.
    /// </summary>
    protected abstract void BuildChartCore();

    /*
    /// <summary>
    /// Gets whether the <see cref="DataSeries"/> supports sampling for large data sets.
    /// </summary>
    public virtual bool SupportsDataSampling
    {
      get { return false; }
    }
    */

    internal virtual void BuildChart()
    {
      if (ItemsSource != null && ItemsSource.Count > 0)
      {
        /*if (SupportsDataSampling)
        {
          SetOptimisationProperties();
        }
        else
        {
          MinimumIndex = CalculateMinimumIndex(0, ItemsSource.Count - 1);
          MaximumIndex = CalculateMaximumIndex(0, ItemsSource.Count - 1);
          IndexStep = 1;
        }*/
        BuildChartCore();
      }
    }

    /// <summary>
    /// Converts the given logical data point to a physical point on the chart canvas.
    /// </summary>
    /// <param name="point">The logical data point to convert.</param>
    /// <returns>A physical position on the chart canvas based on the given logical data point.</returns>
    public Point ConvertLogicalToPhysicalPoint(PolarPoint point)
    {
      if (ThetaAxis == null || RhoAxis == null)
      {
        return new Point();
      }
      double theta = ThetaAxis.ConvertLogicalToPhysical(point.Theta);
      double rho = RhoAxis.ConvertLogicalToPhysical(point.Rho);

      double centerX = Canvas.ActualWidth / 2.0;
      double centerY = Canvas.ActualHeight / 2.0;
      Point vector = GeometryUtils.GetPosition(theta, rho);
      return new Point(centerX + vector.X, centerY - vector.Y);

      //return new Point(Double.IsInfinity(theta) ? 0 : theta, Double.IsInfinity(rho) ? 0 : rho);
    }

    /// <summary>
    /// Gets or sets a binding used to extract the theta axis value from a data object.
    /// </summary>
    public Binding ThetaBinding { get; set; }

    /// <summary>
    /// Gets or sets a binding used to extract the rho axis value from a data object.
    /// </summary>
    public Binding RhoBinding { get; set; }

    private IList<PolarDataPoint> _dataPointMap = new List<PolarDataPoint>();

    /// <summary>
    /// Gets a read only collection containing the <see cref="PolarDataPoint"/> objects within the <see cref="PolarSeries"/>.
    /// A <see cref="PolarDataPoint"/> is the graphical representation of plotted data and is created by the <see cref="PolarSeries"/>.
    /// </summary>
    public ReadOnlyCollection<PolarDataPoint> DataPoints
    {
      get
      {
        return new ReadOnlyCollection<PolarDataPoint>(_dataPointMap);
      }
    }

    /// <summary>
    /// Gets the <see cref="PolarDataPoint"/> mapped to the given data object and index.
    /// </summary>
    /// <param name="o">The data object.</param>
    /// <returns>The <see cref="PolarDataPoint"/> mapped to the given object if it exists.</returns>
    /// <param name="index">The index of the data object within the ItemsSource.</param>
    protected PolarDataPoint GetDataPoint(object o, int index)
    {
      PolarDataPoint dataPoint = null;
      if (index < _dataPointMap.Count && _dataPointMap[index] != null && _dataPointMap[index].DataContext == o)
      {
        dataPoint = _dataPointMap[index];
      }
      if (dataPoint != null)
      {
        dataPoint.ThetaObjectChanged -= new EventHandler(DataPoint_ThetaObjectChanged);
        dataPoint.RhoObjectChanged -= new EventHandler(DataPoint_RhoObjectChanged);
        dataPoint.IsSelectedChangeRequested -= new EventHandler<DataPoint.SelectionChangeRequestArgs>(DataPoint_IsSelectedChangeRequested);
      }
      return dataPoint;
    }

    private PolarDataPoint _selectedDataPoint;

    /// <summary>
    /// Gets the currently selected <see cref="PolarDataPoint"/>.
    /// This will be null if none of the data points in the <see cref="DataSeries"/> are selected.
    /// </summary>
    public PolarDataPoint SelectedDataPoint
    {
      get { return _selectedDataPoint; }
      private set
      {
        if (_selectedDataPoint != value)
        {
          _selectedDataPoint = value;
          OnSelectedDataPointChanged();
          OnPropertyChanged("SelectedDataPoint");
        }
      }
    }

    private ObservableCollection<PolarDataPoint> _selectedDataPoints = new ObservableCollection<PolarDataPoint>();

    // TODO: make this public and implement logic to make sure the selection mode is respected.
    private ObservableCollection<PolarDataPoint> SelectedDataPoints
    {
      get { return _selectedDataPoints; }
    }

    private void OnSelectedDataPointChanged()
    {
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

    /// <summary>
    /// Prepares the given <see cref="PolarDataPoint"/> to participate in selection features and data notifications.
    /// </summary>
    /// <param name="dataPoint">The <see cref="PolarDataPoint"/> to prepare.</param>
    /// <param name="index">The index of the data object within the ItemsSource.</param>
    protected void PrepareDataPoint(PolarDataPoint dataPoint, int index)
    {
      AttachSelectionHandler(dataPoint);
      AttachDataListeners(dataPoint);
      _dataPointMap[index] = dataPoint;
      //dataPoint.CanHighlight = CanHighlight;
    }

    private void AttachDataListeners(PolarDataPoint dataPoint)
    {
      dataPoint.ThetaObjectChanged += new EventHandler(DataPoint_ThetaObjectChanged);
      dataPoint.RhoObjectChanged += new EventHandler(DataPoint_RhoObjectChanged);
    }

    private void DataPoint_RhoObjectChanged(object sender, EventArgs e)
    {
      UpdateLogicalPoint(sender as PolarDataPoint);
      Rebuild();
    }

    private void DataPoint_ThetaObjectChanged(object sender, EventArgs e)
    {
      UpdateLogicalPoint(sender as PolarDataPoint);
      Rebuild();
    }

    private void UpdateLogicalPoint(PolarDataPoint dataPoint)
    {
      double theta = 0;
      double rho = 0;
      if (NumericalUtils.IsPrimitiveNumerical(dataPoint.ThetaObject))
      {
        theta = ThetaAxis.GetLogicalPosition(NumericalUtils.ConvertToDouble(dataPoint.ThetaObject));
      }
      else
      {
        theta = ThetaAxis.GetLogicalPosition(dataPoint.ThetaObject);
      }

      if (NumericalUtils.IsPrimitiveNumerical(dataPoint.RhoObject))
      {
        rho = RhoAxis.GetLogicalPosition(NumericalUtils.ConvertToDouble(dataPoint.RhoObject));
      }
      else
      {
        rho = RhoAxis.GetLogicalPosition(dataPoint.RhoObject);
      }
      dataPoint.LogicalPoint = new PolarPoint(theta, rho);
    }

    private void AttachSelectionHandler(PolarDataPoint dataPoint)
    {
      // Removing handler to ensure we don't stack up.
      dataPoint.IsSelectedChangeRequested -= new EventHandler<DataPoint.SelectionChangeRequestArgs>(DataPoint_IsSelectedChangeRequested);
      dataPoint.IsSelectedChangeRequested += new EventHandler<DataPoint.SelectionChangeRequestArgs>(DataPoint_IsSelectedChangeRequested);
    }

    private void DataPoint_IsSelectedChangeRequested(object sender, DataPoint.SelectionChangeRequestArgs e)
    {
      PolarDataPoint dataPoint = sender as PolarDataPoint;
      switch (SelectionMode)
      {
        case DataPointSelectionMode.Single:
          if (SelectedDataPoint != null)
          {
            SelectedDataPoint.SetIsSelected(false);
          }
          //dataPoint.SetIsSelected(e.RequestedSelection);
          SelectedDataPoint = dataPoint.IsSelected ? dataPoint : null;
          if (dataPoint.IsSelected)
          {
            SelectedDataPoints.Add(dataPoint);
          }
          else
          {
            SelectedDataPoints.Remove(dataPoint);
          }
          break;
        case DataPointSelectionMode.All:
          foreach (PolarDataPoint dp in _dataPointMap)
          {
            dp.SetIsSelected(e.RequestedSelection);
            if (dp.IsSelected)
            {
              SelectedDataPoints.Add(dp);
            }
            else
            {
              SelectedDataPoints.Remove(dp);
            }
          }
          SelectedDataPoint = dataPoint.IsSelected ? dataPoint : null;
          break;
        case DataPointSelectionMode.Multiple:
          //dataPoint.SetIsSelected(e.RequestedSelection);
          if (dataPoint.IsSelected)
          {
            SelectedDataPoints.Add(dataPoint);
            SelectedDataPoint = dataPoint;
          }
          else
          {
            SelectedDataPoints.Remove(dataPoint);
            SelectedDataPoint = SelectedDataPoints.Count == 0 ? null : SelectedDataPoints[0];
          }
          break;
        case DataPointSelectionMode.None:
          e.ActualSelection = false;
          break;
      }
    }

    internal void DeselectAll()
    {
      //foreach (DataPoint dataPoint in _dataPointMap.Values)
      foreach (PolarDataPoint dataPoint in _dataPointMap)
      {
        dataPoint.SetIsSelected(false);
      }
      SelectedDataPoint = null;
    }

    #region Title property

    /// <summary>
    /// Gets or sets the Title.
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
      DependencyProperty.Register("Title", typeof(string), typeof(PolarSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnTitleChanged)));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarSeries)d).OnTitleChanged();
    }

    private void OnTitleChanged()
    {
      OnLegendItemsChanged();
    }

    #endregion // Title property

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
      DependencyProperty.Register("SelectionMode", typeof(DataPointSelectionMode), typeof(PolarSeries),
      new PropertyMetadata(DataPointSelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)));

    private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarSeries)d).OnSelectionModeChanged();
    }

    private void OnSelectionModeChanged()
    {
      foreach (DataPoint dataPoint in _selectedDataPoints)
      {
        dataPoint.SetIsSelected(false);
      }
      _selectedDataPoints.Clear();
      SelectedDataPoint = null;
    }

    #endregion // SelectionMode property
    
    #region LegendIconTemplate property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the legend icon.
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
      DependencyProperty.Register("LegendIconTemplate", typeof(DataTemplate), typeof(PolarSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnLegendIconTemplateChanged)));

    private static void OnLegendIconTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarSeries)d).OnLegendIconTemplateChanged();
    }

    private void OnLegendIconTemplateChanged()
    {
      OnLegendItemsChanged();
    }

    #endregion // LegendIconTemplate property
    
    #region ItemsSource property

    /// <summary>
    /// Gets or sets the ItemsSource. This is the data to be plotted on a <see cref="PolarChart"/> control.
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
      DependencyProperty.Register("ItemsSource", typeof(IList), typeof(PolarSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnItemsSourceChanged)));

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarSeries)d).OnItemsSourceChanged(e);
    }

    private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
    {
      SelectedDataPoint = null;
      _dataPointMap = new List<PolarDataPoint>();
      if (e.NewValue != null)
      {
        // TODO: this is bound to be killing performance. Maybe this can be done in the analyse series method? or only update it when data points are being requested.
        for (int i = 0; i < ((IList)e.NewValue).Count; i++)
        {
          _dataPointMap.Add(null);
        }
      }
      if (e.OldValue != null)
      {
        INotifyCollectionChanged collection = e.OldValue as INotifyCollectionChanged;
        if (collection != null)
        {
          collection.CollectionChanged -= new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
        }
        Rebuild();
      }
      else
      {
        if (ItemsSource.Count > 0)
        {
          AnalyseSeries();
        }
        if (Canvas != null && ThetaAxis != null && RhoAxis != null)
        {
          Rebuild();
          // TODO: this next line is to update the labels if this series has a custom object mapping.
          // Would be nice if we didn't need to call a ChartAxis method from a DataSeries.
          //ThetaAxis.OnPositionsChanged();
          OnLegendItemsChanged();
        }
      }
      INotifyCollectionChanged notifyingCollection = ItemsSource as INotifyCollectionChanged;
      if (notifyingCollection != null)
      {
        notifyingCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
      }
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        for (int i = 0; i < e.OldItems.Count; i++)
        {
          if (_dataPointMap[e.OldStartingIndex] == SelectedDataPoint)
          {
            SelectedDataPoint = null;
          }
          _dataPointMap.RemoveAt(e.OldStartingIndex);
        }
      }
      else if (e.Action == NotifyCollectionChangedAction.Add)
      {
        for (int i = 0; i < e.NewItems.Count; i++)
        {
          _dataPointMap.Insert(e.NewStartingIndex, null);
        }
      }
      else if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        _dataPointMap = new List<PolarDataPoint>();
        for (int i = 0; i < ItemsSource.Count; i++)
        {
          _dataPointMap.Add(null);
        }
        SelectedDataPoint = null;
      }
      Rebuild();
    }

    internal event EventHandler RebuildAll;

    internal void Rebuild()
    {
      EventHandler handler = RebuildAll;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    internal virtual void AnalyseSeries()
    {
      if (ItemsSource != null && ItemsSource.Count > 0 && ((ThetaAxis != null && ThetaAxis.IsAuto) || (RhoAxis != null && RhoAxis.IsAuto)))
      {
        IAxisValueConverter thetaConverter = ThetaAxis.ValueConverter;
        IAxisValueConverter rhoConverter = RhoAxis.ValueConverter;
        bool hasConverter = thetaConverter != null || rhoConverter != null;
        double minTheta = Double.MaxValue;
        double maxTheta = Double.MinValue;
        double minRho = Double.MaxValue;
        double maxRho = Double.MinValue;
        double thetaSpacing = 0;
        double previousTheta = 0;
        int count = 0;

        foreach (object o in ItemsSource)
        {
          PolarPoint point = hasConverter ? GetPoint(o, thetaConverter, rhoConverter) : GetPoint(o);
          if (ReverseAxes)
          {
            point = new PolarPoint(point.Rho, point.Theta);
          }
          minTheta = Math.Min(point.Theta, minTheta);
          maxTheta = Math.Max(point.Theta, maxTheta);
          minRho = Math.Min(point.Rho, minRho);
          maxRho = Math.Max(point.Rho, maxRho);

          if (count >= 2 && thetaSpacing != point.Theta - previousTheta)
          {
            thetaSpacing = 0;
          }
          else if (count > 2 && thetaSpacing > 0)
          {
            thetaSpacing = point.Theta - previousTheta;
          }
          else if (count < 2)
          {
            thetaSpacing = point.Theta - previousTheta;
          }
          previousTheta = point.Theta;
          count++;
        }
        PolarAxisBase dependentAxis = RhoAxis; // ReverseAxes ? ThetaAxis : RhoAxis;
        PolarAxisBase independentAxis = ThetaAxis; // ReverseAxes ? RhoAxis : ThetaAxis;

        if (Series[0] == this)
        {
          if (independentAxis != null && independentAxis.IsAuto)
          {
            independentAxis.Minimum = Math.Round(minTheta);
            independentAxis.Maximum = Math.Round(maxTheta + thetaSpacing);
          }
          if (dependentAxis != null && dependentAxis.IsAuto)
          {
            if (minRho >= 0)
            {
              minRho++;
            }
            dependentAxis.Minimum = Math.Min(0, Math.Floor(minRho) - 1);
            dependentAxis.Maximum = Math.Round(maxRho) + 1;
          }
        }
        else
        {
          if (independentAxis != null && independentAxis.IsAuto)
          {
            independentAxis.Minimum = Math.Min(independentAxis.Minimum, Math.Round(minTheta));
            independentAxis.Maximum = Math.Max(independentAxis.Maximum, Math.Round(maxTheta + thetaSpacing));
          }
          if (dependentAxis != null && dependentAxis.IsAuto)
          {
            if (minRho >= 0)
            {
              minRho++;
            }
            dependentAxis.Minimum = Math.Min(dependentAxis.Minimum, Math.Min(0, Math.Floor(minRho) - 1));
            dependentAxis.Maximum = Math.Max(dependentAxis.Maximum, Math.Round(maxRho) + 1);
          }
        }
      }
    }

    // Faster than the method below since we don't need to null check the converters everytime.
    internal PolarPoint GetPoint(object data)
    {
      PolarPoint point = new PolarPoint();
      if (data is PolarPoint)
      {
        point = (PolarPoint)data;
      }
      else
      {
        //TODO: do something smarter here for object data rather than calling the original GetPoint method.
        PolarDataPoint dataPoint = new PolarDataPoint();
        dataPoint.DataContext = data;
        point = GetPoint(dataPoint, ReverseAxes);
      }
      return point;
    }

    // Extracts the logical values from the data without all the other fancy stuff performed in the other GetPoint methods.
    internal PolarPoint GetPoint(object data, IAxisValueConverter thetaConverter, IAxisValueConverter rhoConverter)
    {
      PolarPoint point = new PolarPoint();
      if (data is PolarPoint)
      {
        point = (PolarPoint)data;
        if (thetaConverter != null)
        {
          point.Theta = thetaConverter.GetAxisPlotPosition(point.Theta);
        }
        if (rhoConverter != null)
        {
          point.Rho = rhoConverter.GetAxisPlotPosition(point.Rho);
        }
      }
      else
      {
        //TODO: do something smarter here for object data rather than calling the original GetPoint method.
        PolarDataPoint dataPoint = new PolarDataPoint();
        dataPoint.DataContext = data;
        point = GetPoint(dataPoint, ReverseAxes);
      }
      return point;
    }

    // TODO: might not need this for polar charts
    internal virtual bool ReverseAxes { get { return false; } }

    #endregion // ItemsSource property

    #region SeriesBrush property

    /// <summary>
    /// Gets or sets the SeriesBrush.
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
      DependencyProperty.Register("SeriesBrush", typeof(Brush), typeof(PolarSeries),
      new PropertyMetadata(new SolidColorBrush(new Color() { A = 255, R = 40, G = 97, B = 169 }), new PropertyChangedCallback(OnSeriesBrushChanged)));

    private static void OnSeriesBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarSeries)d).OnSeriesBrushChanged();
    }

    private void OnSeriesBrushChanged()
    {
      Rebuild();
      OnLegendItemsChanged();
    }

    #endregion // SeriesBrush property

    /// <summary>
    /// Returns a <see cref="PolarPoint"/> containing the theta and rho logical axis values of the given <see cref="DataPoint"/>.
    /// </summary>
    /// <param name="dataPoint">The <see cref="DataPoint"/> holding the data to be plotted.</param>
    /// <returns>A <see cref="PolarPoint"/> containg the theta and rho axis values for the given <see cref="DataPoint"/>.</returns>
    internal PolarPoint GetPoint(PolarDataPoint dataPoint)
    {
      return GetPoint(dataPoint, false);
    }

    /// <summary>
    /// Returns a <see cref="PolarPoint"/> containing the theta and rho logical axis values of the given <see cref="DataPoint"/>.
    /// </summary>
    /// <param name="dataPoint">The <see cref="PolarDataPoint"/> holding the data to calculate a <see cref="PolarPoint"/> for.</param>
    /// <param name="reverseAxes">Specifies whether or not the RhoAxis is the independent axis.</param>
    /// <returns>A <see cref="PolarPoint"/> containg the theta and rho axis values for the given data object.</returns>
    internal PolarPoint GetPoint(PolarDataPoint dataPoint, bool reverseAxes)
    {
      if (dataPoint.DataContext is PolarPoint && ThetaBinding == null && RhoBinding == null)
      {
        PolarPoint result = (PolarPoint)dataPoint.DataContext;
        double resultTheta = ThetaAxis.GetLogicalPosition(result.Theta);
        double resultRho = RhoAxis.GetLogicalPosition(result.Rho);
        result = new PolarPoint(resultTheta, resultRho);
        if (ThetaAxis != null)
        {
          dataPoint.ThetaObject = GetObject(ThetaAxis.GetLabelString(ThetaAxis.LabelFormat, ThetaAxis.GetLabel(result.Theta)));
        }
        if (RhoAxis != null)
        {
          dataPoint.RhoObject = GetObject(RhoAxis.GetLabelString(RhoAxis.LabelFormat, RhoAxis.GetLabel(result.Rho)));
        }
        dataPoint.LogicalPoint = result;
        return result;
      }
      if (ThetaBinding != null)
      {
        dataPoint.SetBinding(PolarDataPoint.ThetaObjectProperty, ThetaBinding);
      }
      if (RhoBinding != null)
      {
        dataPoint.SetBinding(PolarDataPoint.RhoObjectProperty, RhoBinding);
      }

      object o = dataPoint.DataContext;
      double theta = 0;
      if (ThetaBinding == null)
      {
        if (reverseAxes)
        {
          if (NumericalUtils.IsPrimitiveNumerical(o))
          {
            theta = NumericalUtils.ConvertToDouble(o) ?? 0;
            theta = ThetaAxis.GetLogicalPosition(theta);
          }
          else
          {
            theta = ThetaAxis.GetLogicalPosition(o);
          }
        }
        else
        {
          theta = ItemsSource.IndexOf(o);
        }

        dataPoint.ThetaObject = GetObject(ThetaAxis.GetLabelString(ThetaAxis.LabelFormat, ThetaAxis.GetLabel(theta)));
      }
      else if (NumericalUtils.IsPrimitiveNumerical(dataPoint.ThetaObject))
      {
        theta = NumericalUtils.ConvertToDouble(dataPoint.ThetaObject) ?? 0;
        theta = ThetaAxis.GetLogicalPosition(theta);
      }
      else if (dataPoint.ThetaObject != null)
      {
        /*
        // The optimisation routine doesn't work when category axis support is being used.
        // This condition detects that category axis support is being used and helps disable the optimising routine.
        if (_optimising && !NumericalUtils.IsPrimitiveNumerical(dataPoint.ThetaObject))
        {
          theta = Double.NaN;
        }
        else*/
        {
          theta = ThetaAxis.GetLogicalPosition(dataPoint.ThetaObject);
        }
      }

      double rho = 0;
      if (RhoBinding == null)
      {
        if (reverseAxes)
        {
          rho = ItemsSource.IndexOf(o);
        }
        else
        {
          if (NumericalUtils.IsPrimitiveNumerical(o))
          {
            rho = NumericalUtils.ConvertToDouble(o) ?? 0;
            rho = RhoAxis.GetLogicalPosition(rho);
          }
          else
          {
            rho = RhoAxis.GetLogicalPosition(o);
          }
        }
        dataPoint.RhoObject = GetObject(RhoAxis.GetLabelString(RhoAxis.LabelFormat, RhoAxis.GetLabel(rho)));
      }
      else if (NumericalUtils.IsPrimitiveNumerical(dataPoint.RhoObject))
      {
        rho = NumericalUtils.ConvertToDouble(dataPoint.RhoObject) ?? 0;
        rho = RhoAxis.GetLogicalPosition(rho);
      }
      else if (dataPoint.RhoObject != null)
      {
        rho = RhoAxis.GetLogicalPosition(dataPoint.RhoObject);
      }

      dataPoint.LogicalPoint = new PolarPoint(theta, rho);
      return new PolarPoint(theta, rho);
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

    /// <summary>
    /// Raised when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}

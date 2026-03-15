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
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Infralution.Licensing;
using System.Reflection;
using System.Windows.Data;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A control for display polar coordinate data series.
  /// </summary>
  [ContentProperty("Series")]
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class PolarChart : Control
  {
    private Canvas _canvas = new Canvas(); // This intial Canvas instance is useful when working in testing environments.

    private ObservableCollection<PolarSeries> _series;
    private ObservableCollection<UIElement> _foregroundElements = new ObservableCollection<UIElement>();
    private ObservableCollection<UIElement> _backgroundElements = new ObservableCollection<UIElement>();
    private readonly LegendItemCollection _legendItems = new LegendItemCollection();
    private readonly PolarChartGrid _defaultGrid = new PolarChartGrid();

    private bool _isMouseDown;
    private bool _isMouseOver;

    static PolarChart()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarChart),
        new FrameworkPropertyMetadata(typeof(PolarChart)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChart"/> class.
    /// </summary>
    public PolarChart()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      _series = new ObservableCollection<PolarSeries>();
      _series.CollectionChanged += new NotifyCollectionChangedEventHandler(Series_CollectionChanged);

      _backgroundElements.Add(_defaultGrid);
      _backgroundElements.CollectionChanged += new NotifyCollectionChangedEventHandler(BackgroundElements_CollectionChanged);

      Loaded += new RoutedEventHandler(PolarChart_Loaded);
    }

    private void PolarChart_Loaded(object sender, RoutedEventArgs e)
    {
      if (ThetaAxis == null)
      {
        ThetaAxis = new ThetaAxis();
      }
      else if (ThetaAxis == _defaultThetaAxis && BindingOperations.GetBinding(this, ThetaAxisProperty) == null)
      {
        ThetaAxis = _defaultThetaAxis;
      }
      if (ThetaAxis.Maximum == ThetaAxis.Minimum)
      {
        ThetaAxis.IsAuto = true;
      }

      if (RhoAxis == null)
      {
        RhoAxis = new RhoAxis();
      }
      else if (RhoAxis == _defaultRhoAxis && BindingOperations.GetBinding(this, RhoAxisProperty) == null)
      {
        RhoAxis = _defaultRhoAxis;
      }
      if (RhoAxis.Maximum == RhoAxis.Minimum)
      {
        RhoAxis.IsAuto = true;
      }
      RebuildAll();
    }

    private void BackgroundElements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        Dispatcher.BeginInvoke(new Action(RemoveDefaultGrid));
      }
    }

    private void RemoveDefaultGrid()
    {
      _backgroundElements.Remove(_defaultGrid);
    }

    private void Series_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (PolarSeries series in e.OldItems)
        {
          RemoveSeriesEventHandlers(series);
        }
      }
      if (e.NewItems != null)
      {
        foreach (PolarSeries series in e.NewItems)
        {
          series.Series = Series;
          series.ThetaAxis = ThetaAxis;
          series.RhoAxis = RhoAxis; // GetRhoAxis(series.RhoAxisTitle);
          series.Canvas = _canvas;
          AttachSeriesEventHandlers(series);
          if (series.Title == null && series.GetBindingExpression(PolarSeries.TitleProperty) == null)
          {
            series.Title = "Series " + (Series.IndexOf(series) + 1);
          }
        }
      }
      _legendItems.Reload(this);
      RebuildAll();
    }

    private void DataSeries_RebuildAll(object sender, EventArgs e)
    {
      RebuildAll();
    }

    private void RebuildAll()
    {
      //if (Visibility == Visibility.Visible)// && _isLoaded)
      {
        if (_canvas != null)
        {
          _canvas.Children.Clear();
        }
        foreach (PolarSeries series in Series)
        {
          series.AnalyseSeries();
        }
        if (_canvas != null)
        {
          _canvas.Children.Clear();
          foreach (PolarSeries series in Series)
          {
            series.BuildChart();
          }
        }
        //AddWatermark();
      }
    }

    private void AttachSeriesEventHandlers(PolarSeries series)
    {
      series.LegendItemsChanged += new EventHandler(Series_LegendItemsChanged);
      series.RebuildAll += new EventHandler(DataSeries_RebuildAll);
      series.SelectedDataPointChanged += new EventHandler<SelectedDataPointChangedEventArgs>(DataSeries_SelectedDataPointChanged);
    }

    private void RemoveSeriesEventHandlers(PolarSeries series)
    {
      series.LegendItemsChanged += new EventHandler(Series_LegendItemsChanged);
      series.RebuildAll -= new EventHandler(DataSeries_RebuildAll);
      series.SelectedDataPointChanged -= new EventHandler<SelectedDataPointChangedEventArgs>(DataSeries_SelectedDataPointChanged);
    }

    private void Series_LegendItemsChanged(object sender, EventArgs e)
    {
      _legendItems.Reload(this);
    }

    private PolarSeries _selectedDataSeries;

    private void DataSeries_SelectedDataPointChanged(object sender, SelectedDataPointChangedEventArgs e)
    {
      PolarSeries series = sender as PolarSeries;
      if (_selectedDataSeries != null && series != _selectedDataSeries && SelectionMode == DataSeriesSelectionMode.Single)
      {
        if (series.SelectedDataPoint != null && series.SelectedDataPoint.IsSelected)
        {
          _selectedDataSeries.DeselectAll();
        }
      }
      if (_selectedDataSeries == series && series.SelectedDataPoint == null)
      {
        _selectedDataSeries = null;
      }
      else if (series.SelectedDataPoint != null && series.SelectedDataPoint.IsSelected)
      {
        _selectedDataSeries = series;
      }
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _canvas.Children.Clear();
      _canvas = GetTemplateChild("PART_ChartCanvas") as Canvas;
      AttachCanvasEventHandlers(_canvas);

      foreach (PolarSeries series in _series)
      {
        series.ThetaAxis = ThetaAxis;
        series.RhoAxis = RhoAxis; // GetRhoAxis(series.RhoAxisTitle);
        series.Canvas = _canvas;
      }      
    }

    private void AttachCanvasEventHandlers(Canvas canvas)
    {
      canvas.MouseLeftButtonDown += new MouseButtonEventHandler(Canvas_MouseLeftButtonDown);
      canvas.MouseLeftButtonUp += new MouseButtonEventHandler(Canvas_MouseLeftButtonUp);
      canvas.MouseEnter += new MouseEventHandler(Canvas_MouseEnter);
      canvas.MouseLeave += new MouseEventHandler(Canvas_MouseLeave);
      canvas.MouseMove += new MouseEventHandler(Canvas_MouseMove);
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
      OnChartMouseMove(CreatePolarMouseEventArgs(e));
    }

    private void Canvas_MouseLeave(object sender, MouseEventArgs e)
    {
      _isMouseOver = false;
      OnChartMouseLeave(CreatePolarMouseEventArgs(e));
    }

    private void Canvas_MouseEnter(object sender, MouseEventArgs e)
    {
      _isMouseOver = true;
      OnChartMouseEnter(CreatePolarMouseEventArgs(e));
    }

    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      _isMouseDown = false;
      OnChartMouseLeftButtonUp(CreatePolarMouseEventArgs(e));
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _isMouseDown = true;
      OnChartMouseLeftButtonDown(CreatePolarMouseEventArgs(e));
    }

    private PolarChartMouseEventArgs CreatePolarMouseEventArgs(MouseEventArgs e)
    {
      Point position = e.GetPosition(_canvas);
      Point center = new Point(_canvas.ActualWidth / 2.0, _canvas.ActualHeight / 2.0);
      double angle = GeometryUtils.GetAngle(new Point(position.X - center.X, position.Y - center.Y));
      double dist = GeometryUtils.Distance(center, position);
      double logicalTheta = ThetaAxis.ConvertPhysicalToLogical(angle);
      double logicalRho = RhoAxis.ConvertPhysicalToLogical(dist);

      // TODO: _isMouseOver should really include the rho constraint logic seen at the end of this next line:
      return new PolarChartMouseEventArgs(position, new PolarPoint(angle, dist), new PolarPoint(logicalTheta, logicalRho), BuildConstrainedLogicalPoint(logicalTheta, logicalRho), _isMouseDown, _isMouseOver && logicalRho <= RhoAxis.Maximum);
    }

    private PolarPoint BuildConstrainedLogicalPoint(double logicalTheta, double logicalRho)
    {
      return new PolarPoint(logicalTheta, Math.Max(RhoAxis.Minimum, Math.Min(RhoAxis.Maximum, logicalRho)));
    }

    /// <summary>
    /// Raised when the mouse is moved over the chart area.
    /// </summary>
    public event EventHandler<PolarChartMouseEventArgs> ChartMouseMove;

    /// <summary>
    /// Raised when the mouse enters the chart area.
    /// </summary>
    public event EventHandler<PolarChartMouseEventArgs> ChartMouseEnter;

    /// <summary>
    /// Raised when the mouse leaves the chart area.
    /// </summary>
    public event EventHandler<PolarChartMouseEventArgs> ChartMouseLeave;

    /// <summary>
    /// Raised when the left mouse button is released over the the chart area.
    /// </summary>
    public event EventHandler<PolarChartMouseEventArgs> ChartMouseLeftButtonUp;

    /// <summary>
    /// Raised when the left mouse button is pressed over the chart area.
    /// </summary>
    public event EventHandler<PolarChartMouseEventArgs> ChartMouseLeftButtonDown;

    private void OnChartMouseLeftButtonDown(PolarChartMouseEventArgs args)
    {
      EventHandler<PolarChartMouseEventArgs> handler = ChartMouseLeftButtonDown;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseLeftButtonUp(PolarChartMouseEventArgs args)
    {
      EventHandler<PolarChartMouseEventArgs> handler = ChartMouseLeftButtonUp;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseEnter(PolarChartMouseEventArgs args)
    {
      EventHandler<PolarChartMouseEventArgs> handler = ChartMouseEnter;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseLeave(PolarChartMouseEventArgs args)
    {
      EventHandler<PolarChartMouseEventArgs> handler = ChartMouseLeave;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseMove(PolarChartMouseEventArgs args)
    {
      EventHandler<PolarChartMouseEventArgs> handler = ChartMouseMove;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    /// <summary>
    /// Gets the collection of the <see cref="DataSeries"/> plotted on the chart.
    /// </summary>
    public ObservableCollection<PolarSeries> Series
    {
      get { return _series; }
    }

    /// <summary>
    /// Gets the items to be displayed in the legend.
    /// </summary>
    public ReadOnlyCollection<LegendItem> LegendItems
    {
      get { return _legendItems; }
    }

    /// <summary>
    /// Gets the collection of <see cref="UIElement"/> objects displayed in front of the chart data.
    /// </summary>
    public Collection<UIElement> ForegroundElements
    {
      get { return _foregroundElements; }
    }

    /// <summary>
    /// Gets the collection of <see cref="UIElement"/> objects displayed behind the chart data.
    /// </summary>
    public Collection<UIElement> BackgroundElements
    {
      get { return _backgroundElements; }
    }

    #region ThetaAxis property

    private ThetaAxis _defaultThetaAxis;

    /// <summary>
    /// Gets or sets the <see cref="ThetaAxis"/> of this <see cref="PolarChart"/>.
    /// This is a dependency property.
    /// </summary>
    public ThetaAxis ThetaAxis
    {
      get
      {
        ThetaAxis axis = (ThetaAxis)GetValue(ThetaAxisProperty);
        if (axis == null)
        {
          if (_defaultThetaAxis == null)
          {
            _defaultThetaAxis = new ThetaAxis() { IsAuto = true };
            PrepareThetaAxis(_defaultThetaAxis);
          }
          axis = _defaultThetaAxis;
        }
        else
        {
          if (_defaultThetaAxis != null)
          {
            RemoveAxisEventHandlers(_defaultThetaAxis);
            _defaultThetaAxis = null;
          }
        }
        return axis;
      }
      set { SetValue(ThetaAxisProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ThetaAxis"/> property.
    /// </summary>
    public static readonly DependencyProperty ThetaAxisProperty =
      DependencyProperty.Register("ThetaAxis", typeof(ThetaAxis), typeof(PolarChart),
      new PropertyMetadata(new PropertyChangedCallback(OnThetaAxisChanged)));

    private static void OnThetaAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChart)d).OnThetaAxisChanged(e);
    }

    private void OnThetaAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      ThetaAxis oldAxis = e.OldValue as ThetaAxis;
      if (oldAxis != null)
      {
        RemoveAxisEventHandlers(oldAxis);
      }
      if (ThetaAxis != null)
      {
        PrepareThetaAxis(ThetaAxis);
      }
    }

    private void PrepareThetaAxis(ThetaAxis axis)
    {
      if (axis.Minimum == axis.Maximum && IsLoaded)
      {
        axis.IsAuto = true;
      }
      AttachAxisEventHandlers(axis);
      foreach (PolarSeries series in Series)
      {
        series.ThetaAxis = axis;
      }
    }

    #endregion // ThetaAxis property

    #region RhoAxis property

    private RhoAxis _defaultRhoAxis;

    /// <summary>
    /// Gets or sets the <see cref="RhoAxis"/> of this <see cref="PolarChart"/>.
    /// This is a dependency property.
    /// </summary>
    public RhoAxis RhoAxis
    {
      get
      {
        RhoAxis axis = (RhoAxis)GetValue(RhoAxisProperty);
        if (axis == null)
        {
          if (_defaultRhoAxis == null)
          {
            _defaultRhoAxis = new RhoAxis() { IsAuto = true };
            PrepareRhoAxis(_defaultRhoAxis);
          }
          axis = _defaultRhoAxis;
        }
        else
        {
          if (_defaultRhoAxis != null)
          {
            RemoveAxisEventHandlers(_defaultRhoAxis);
            _defaultRhoAxis = null;
          }
        }
        return axis;
      }
      set { SetValue(RhoAxisProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RhoAxis"/> property.
    /// </summary>
    public static readonly DependencyProperty RhoAxisProperty =
      DependencyProperty.Register("RhoAxis", typeof(RhoAxis), typeof(PolarChart),
      new PropertyMetadata(new PropertyChangedCallback(OnRhoAxisChanged)));

    private static void OnRhoAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChart)d).OnRhoAxisChanged(e);
    }

    private void OnRhoAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      RhoAxis oldAxis = e.OldValue as RhoAxis;
      if (oldAxis != null)
      {
        RemoveAxisEventHandlers(oldAxis);
      }
      if (RhoAxis != null)
      {
        PrepareRhoAxis(RhoAxis);
      }
    }

    private void PrepareRhoAxis(RhoAxis axis)
    {
      if (axis.Minimum == axis.Maximum && IsLoaded)
      {
        axis.IsAuto = true;
      }
      AttachAxisEventHandlers(axis);
      foreach (PolarSeries series in Series)
      {
        series.RhoAxis = axis;
      }
    }

    #endregion // RhoAxis property

    private void AttachAxisEventHandlers(PolarAxisBase axis)
    {
      axis.AxisUpdatedInternal += new EventHandler(Axis_AxisUpdatedInternal);
    }

    private void RemoveAxisEventHandlers(PolarAxisBase axis)
    {
      axis.AxisUpdatedInternal -= new EventHandler(Axis_AxisUpdatedInternal);
    }

    private void Axis_AxisUpdatedInternal(object sender, EventArgs e)
    {
      //BuildChart();
      BuildChartLater();
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
      DependencyProperty.Register("Title", typeof(string), typeof(PolarChart),
      new PropertyMetadata(new PropertyChangedCallback(OnTitleChanged)));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChart)d).OnTitleChanged();
    }

    private void OnTitleChanged()
    {
    }

    #endregion // Title property

    #region TitleTemplate property

    /// <summary>
    /// Gets or sets the TitleTemplate.
    /// This is a dependency property.
    /// </summary>
    public DataTemplate TitleTemplate
    {
      get { return (DataTemplate)GetValue(TitleTemplateProperty); }
      set { SetValue(TitleTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TitleTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleTemplateProperty =
      DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(PolarChart),
      new PropertyMetadata(new PropertyChangedCallback(OnTitleTemplateChanged)));

    private static void OnTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChart)d).OnTitleTemplateChanged();
    }

    private void OnTitleTemplateChanged()
    {
    }

    #endregion // TitleTemplate property

    #region LegendStyle property

    /// <summary>
    /// Gets or sets the LegendStyle.
    /// This is a dependency property.
    /// </summary>
    public Style LegendStyle
    {
      get { return (Style)GetValue(LegendStyleProperty); }
      set { SetValue(LegendStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LegendStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty LegendStyleProperty =
      DependencyProperty.Register("LegendStyle", typeof(Style), typeof(PolarChart),
      new PropertyMetadata(GetDefaultLegendStyle(), new PropertyChangedCallback(OnLegendStyleChanged)));

    private static void OnLegendStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChart)d).OnLegendStyleChanged();
    }

    private void OnLegendStyleChanged()
    {
    }

    private static Style GetDefaultLegendStyle()
    {
      Style centerLegendStyle = new Style(typeof(Legend));
      centerLegendStyle.Setters.Add(new Setter(Legend.HorizontalAlignmentProperty, HorizontalAlignment.Right));
      centerLegendStyle.Setters.Add(new Setter(Legend.VerticalAlignmentProperty, VerticalAlignment.Top));
      return centerLegendStyle;
    }

    #endregion // LegendStyle property

    #region LegendPosition property

    /// <summary>
    /// Gets or sets the LegendPosition.
    /// This is a dependency property.
    /// </summary>
    public LegendPosition LegendPosition
    {
      get { return (LegendPosition)GetValue(LegendPositionProperty); }
      set { SetValue(LegendPositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LegendPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty LegendPositionProperty =
      DependencyProperty.Register("LegendPosition", typeof(LegendPosition), typeof(PolarChart),
      new PropertyMetadata(LegendPosition.Center, new PropertyChangedCallback(OnLegendPositionChanged)));

    private static void OnLegendPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChart)d).OnLegendPositionChanged();
    }

    private bool _isUsingDefaultCenterLegendStyle = true;

    private void OnLegendPositionChanged()
    {
      if (LegendPosition == LegendPosition.Center && LegendStyle == null)
      {
        _isUsingDefaultCenterLegendStyle = true;
        Style centerLegendStyle = new Style(typeof(Legend));
        centerLegendStyle.Setters.Add(new Setter(Legend.HorizontalAlignmentProperty, HorizontalAlignment.Right));
        centerLegendStyle.Setters.Add(new Setter(Legend.VerticalAlignmentProperty, VerticalAlignment.Top));
        LegendStyle = centerLegendStyle;
      }
      else if (LegendPosition != LegendPosition.Center && _isUsingDefaultCenterLegendStyle)
      {
        LegendStyle = null;
      }
    }

    #endregion // LegendPosition property

    #region SelectionMode property

    /// <summary>
    /// Gets or sets the selection mode of the <see cref="PolarChart"/>.
    /// This is a dependency property.
    /// </summary>
    public DataSeriesSelectionMode SelectionMode
    {
      get { return (DataSeriesSelectionMode)GetValue(SelectionModeProperty); }
      set { SetValue(SelectionModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectionMode"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectionModeProperty =
      DependencyProperty.Register("SelectionMode", typeof(DataSeriesSelectionMode), typeof(PolarChart),
      new PropertyMetadata(DataSeriesSelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)));

    private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarChart)d).OnSelectionModeChanged();
    }

    private void OnSelectionModeChanged()
    {
      if (SelectionMode == DataSeriesSelectionMode.Single)
      {
        foreach (PolarSeries series in Series)
        {
          series.DeselectAll();
        }
      }
    }

    #endregion // SelectionMode property
    
    private bool _buildChartQueued;

    private void BuildChartLater()
    {
      if (!_buildChartQueued)
      {
        _buildChartQueued = true;
        Dispatcher.BeginInvoke(new Action(BuildChart));
      }
    }

    private void BuildChart()
    {
      _canvas.Children.Clear();
      if (Visibility == Visibility.Visible)
      {
        foreach (PolarSeries series in Series)
        {
          series.BuildChart();
        }
        //AddWatermark();
      }
      _buildChartQueued = false;
    }
  }
}

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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Markup;
using System.Linq;
using System.Diagnostics;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A control for displaying <see cref="PieSeries"/>.
  /// </summary>
  [ContentProperty("Series")]
  [TemplatePart(Name = "PART_ChartCanvas", Type = typeof(Canvas))]
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class PieChart : Control
  {
    private Canvas _canvas = new Canvas() { Width = 100, Height = 100 }; // This Canvas instance is useful when working in testing environments.
    private ObservableCollection<PieSeries> _series = new ObservableCollection<PieSeries>();
    private LegendItemCollection _legendItems = new LegendItemCollection();

    static PieChart()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PieChart),
        new FrameworkPropertyMetadata(typeof(PieChart)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart"/> class.
    /// </summary>
    public PieChart()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      _series.CollectionChanged += new NotifyCollectionChangedEventHandler(Series_CollectionChanged);
    }

    private void Series_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (PieSeries series in e.NewItems)
        {
          series.LegendItemsChanged += new EventHandler(PieSeries_LegendItemsChanged);
          series.RebuildAll += new EventHandler(PieSeries_RebuildAll);
          series.SelectedDataPointChanged += new EventHandler<SelectedDataPointChangedEventArgs>(Series_SelectedDataPointChanged);
          series.Canvas = _canvas;
          series.PieChart = this;
        }
      }
      _legendItems.Reload(this);
      Rebuild();
    }

    private void Series_SelectedDataPointChanged(object sender, SelectedDataPointChangedEventArgs e)
    {
      PieSeries series = sender as PieSeries;
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

    private PieSeries _selectedDataSeries;

    private void PieSeries_RebuildAll(object sender, EventArgs e)
    {
      Rebuild();
    }

    private void PieSeries_LegendItemsChanged(object sender, EventArgs e)
    {
      _legendItems.Reload(this);
    }

    /// <summary>
    /// Gets the collection of <see cref="PieSeries"/> displayed by the <see cref="PieChart"/>.
    /// </summary>
    public Collection<PieSeries> Series { get { return _series; } }

    /// <summary>
    /// Gets the items to be displayed in the legend.
    /// </summary>
    public ReadOnlyCollection<LegendItem> LegendItems
    {
      get { return _legendItems; }
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _canvas.Children.Clear();
      _canvas = GetTemplateChild("PART_ChartCanvas") as Canvas;
      _canvas.SizeChanged += new SizeChangedEventHandler(Canvas_SizeChanged);
      double seriesCount = _series.Count;
      double index = seriesCount;
      foreach (PieSeries series in Series)
      {
        series.Canvas = _canvas;
        series.MainRadiusPercentage = index * (1 / seriesCount);
        index--;
      }
    }

    private void Rebuild()
    {
      if (_canvas != null)
      {
        _canvas.Children.Clear();
        double seriesCount = _series.Count;
        double index = seriesCount;
        foreach (PieSeries series in Series)
        {
          series.MainRadiusPercentage = index * (1 / seriesCount);
          index--;
          series.BuildChart();
        }
        AddWatermark();
      }
    }

    [Conditional("TRIAL")]
    private void AddWatermark()
    {
      Watermark.AddWatermark(_canvas, true);
    }

    private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      Rebuild();
    }

    #region Title property

    /// <summary>
    /// Gets or sets the title of the <see cref="PieChart"/>.
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
      DependencyProperty.Register("Title", typeof(string), typeof(PieChart),
      new PropertyMetadata(null));

    #endregion // Title property

    #region TitleTemplate property

    /// <summary>
    /// Gets or sets the DataTemplate applied to the title of the <see cref="PieChart"/>.
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
      DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(PieChart),
      new PropertyMetadata(null));

    #endregion // TitleTemplate property

    #region SelectionMode property

    /// <summary>
    /// Gets or sets the selection mode of the <see cref="PieChart"/>.
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
      DependencyProperty.Register("SelectionMode", typeof(DataSeriesSelectionMode), typeof(PieChart),
      new PropertyMetadata(DataSeriesSelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)));

    private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieChart)d).OnSelectionModeChanged();
    }

    private void OnSelectionModeChanged()
    {
      if (SelectionMode == DataSeriesSelectionMode.Single)
      {
        foreach (PieSeries series in Series)
        {
          series.DeselectAll();
        }
      }
    }

    #endregion // SelectionMode property

    #region LegendStyle property

    /// <summary>
    /// Gets or sets the Style applied to the HeaderedItemsControl that displays the legend items.
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
      DependencyProperty.Register("LegendStyle", typeof(Style), typeof(PieChart),
      new PropertyMetadata(null));

    #endregion // LegendStyle property

    #region LegendPosition property

    /// <summary>
    /// Gets or sets the position on the <see cref="PieChart"/> control where the legend is displayed.
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
      DependencyProperty.Register("LegendPosition", typeof(LegendPosition), typeof(PieChart),
      new PropertyMetadata(LegendPosition.Right, new PropertyChangedCallback(OnLegendPositionChanged)));

    private static void OnLegendPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieChart)d).OnLegendPositionChanged();
    }

    private bool _isUsingDefaultCenterLegendStyle;

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

    #region RadiusFactor Property

    /// <summary>
    /// Gets or sets the radius factor.
    /// This is a value between 0 and 1 which specifies the proportional size of the rendered data compared to the available space in the rendering area.
    /// The default value is 0.8.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RadiusFactorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double RadiusFactor
    {
      get { return (double)GetValue(RadiusFactorProperty); }
      set { SetValue(RadiusFactorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RadiusFactor"/> property.
    /// </summary>
    public static readonly DependencyProperty RadiusFactorProperty =
      DependencyProperty.Register("RadiusFactor", typeof(double), typeof(PieChart),
      new FrameworkPropertyMetadata(0.8, OnRadiusFactorChanged));

    private static void OnRadiusFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieChart)d).OnRadiusFactorChanged();
    }

    private void OnRadiusFactorChanged()
    {
      Rebuild();
    }

    #endregion // RadiusFactor Property
  }
}

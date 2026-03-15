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
using System.Windows.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.IO;
using System.Xml;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Holds data for a single pie chart series.
  /// </summary>
  public class PieSeries : Control
  {
    private Canvas _canvas;
    private ObservableCollection<Brush> _brushes = new ObservableCollection<Brush>();
    private Binding _dataBinding, _radiusBinding;
    private bool _brushesLock;

    private Dictionary<IndexData, PieSlice> _dataPointMap = new Dictionary<IndexData, PieSlice>();

    static PieSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PieSeries),
        new FrameworkPropertyMetadata(typeof(PieSeries)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PieSeries"/> class.
    /// </summary>
    public PieSeries()
    {
      _brushes.CollectionChanged += new NotifyCollectionChangedEventHandler(Brushes_CollectionChanged);
      MainRadiusPercentage = 1;
    }

    private void Brushes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      Rebuild();
      OnLegendItemsChanged();
    }

    private void FillBrushes()
    {
      if (!_brushesLock)
      {
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 15, G = 111, B = 198 }));
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 11, G = 208, B = 217 }));
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 124, G = 202, B = 98 }));
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 165, G = 194, B = 73 }));
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 56, G = 112, B = 37 }));
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 0, G = 158, B = 217 }));
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 7, G = 55, B = 99 }));
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 84, G = 168, B = 56 }));
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 16, G = 207, B = 255 }));
        _brushes.Add(new SolidColorBrush(new Color() { A = 255, R = 7, G = 103, B = 77 }));
        _brushesLock = true;
      }
    }

    /// <summary>
    /// Gets a read only collection containing the <see cref="PieSlice"/> objects within the <see cref="PieSeries"/>.
    /// A <see cref="PieSlice"/> is the graphical representation of plotted data and is created by the <see cref="PieSeries"/>.
    /// </summary>
    public ReadOnlyCollection<PieSlice> DataPoints
    {
      get
      {
        IList<PieSlice> points = new List<PieSlice>();
        foreach (PieSlice point in _dataPointMap.Values)
        {
          points.Add(point);
        }
        return new ReadOnlyCollection<PieSlice>(points);
      }
    }

    /// <summary>
    /// Gets the observable collection of brushes used to color each piece of the pie chart.
    /// </summary>
    public ObservableCollection<Brush> Brushes
    {
      get { return _brushes; }
    }

    internal double MainRadiusPercentage { get; set; }

    internal PieChart PieChart { get; set; }

    internal event EventHandler RebuildAll;

    private void Rebuild()
    {
      EventHandler handler = RebuildAll;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    #region ItemsSource property

    /// <summary>
    /// Gets or sets the ItemsSource. This is the data that the <see cref="PieChart"/> displays.
    /// If the sum of the given data values is less than 1, then a partial pie chart is created.
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
      DependencyProperty.Register("ItemsSource", typeof(IList), typeof(PieSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnItemsSourceChanged)));

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnItemsSourceChanged(e);
    }

    private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
    {
      _selectedSlice = null;
      _dataPointMap = new Dictionary<IndexData, PieSlice>();
      if (e.OldValue == null)
      {
        BuildChart();
      }
      else
      {
        INotifyCollectionChanged oldNotify = e.OldValue as INotifyCollectionChanged;
        if (oldNotify != null)
        {
          oldNotify.CollectionChanged -= new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
        }
        Rebuild();
      }
      OnLegendItemsChanged();

      INotifyCollectionChanged notify = ItemsSource as INotifyCollectionChanged;
      if (notify != null)
      {
        notify.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsSource_CollectionChanged);
      }
    }

    private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      Rebuild();
      OnLegendItemsChanged();
    }

    #endregion // ItemsSource property

    #region IsSingleDataLineVisible property

    /// <summary>
    /// Gets or sets whether or not to display the line from the center to the edge when there is only a single piece of data.
    /// This is a dependency property.
    /// </summary>
    public bool IsSingleDataLineVisible
    {
      get { return (bool)GetValue(IsSingleDataLineVisibleProperty); }
      set { SetValue(IsSingleDataLineVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsSingleDataLineVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSingleDataLineVisibleProperty =
      DependencyProperty.Register("IsSingleDataLineVisible", typeof(bool), typeof(PieSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnIsSingleDataLineVisibleChanged)));

    private static void OnIsSingleDataLineVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnIsSingleDataLineVisibleChanged();
    }

    private void OnIsSingleDataLineVisibleChanged()
    {
    }

    #endregion // IsSingleDataLineVisible property

    #region DoughnutScale property

    /// <summary>
    /// Gets or sets a value used to determine the thickness of a doughnut. This should be a value between 0 and 1.
    /// A value of 1 will create a pie chart. The default is 1.
    /// This is a dependency property.
    /// </summary>
    public double DoughnutScale
    {
      get { return (double)GetValue(DoughnutScaleProperty); }
      set { SetValue(DoughnutScaleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DoughnutScale"/> property.
    /// </summary>
    public static readonly DependencyProperty DoughnutScaleProperty =
      DependencyProperty.Register("DoughnutScale", typeof(double), typeof(PieSeries),
      new PropertyMetadata(1.0, new PropertyChangedCallback(OnDoughnutScaleChanged)));

    private static void OnDoughnutScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnDoughnutScaleChanged();
    }

    private void OnDoughnutScaleChanged()
    {
      Rebuild();
    }

    #endregion // DoughnutScale property

    #region ShowDataLabels property

    /// <summary>
    /// Gets or sets whether or not the data lables are displayed.
    /// This is a dependency property.
    /// </summary>
    public bool ShowDataLabels
    {
      get { return (bool)GetValue(ShowDataLabelsProperty); }
      set { SetValue(ShowDataLabelsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowDataLabels"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowDataLabelsProperty =
      DependencyProperty.Register("ShowDataLabels", typeof(bool), typeof(PieSeries),
      new PropertyMetadata(false, new PropertyChangedCallback(OnShowDataLabelsChanged)));

    private static void OnShowDataLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnShowDataLabelsChanged();
    }

    private void OnShowDataLabelsChanged()
    {
      // TODO: update the data labels without rebuilding the entire chart.
      Rebuild();
    }

    #endregion // ShowDataLabels property

    #region ShowDataLabelLines property

    /// <summary>
    /// Gets or sets whether or not to display a line from the data labels to the data pieces.
    /// This is a dependency property.
    /// </summary>
    public bool ShowDataLabelLines
    {
      get { return (bool)GetValue(ShowDataLabelLinesProperty); }
      set { SetValue(ShowDataLabelLinesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowDataLabelLines"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowDataLabelLinesProperty =
      DependencyProperty.Register("ShowDataLabelLines", typeof(bool), typeof(PieSeries),
      new PropertyMetadata(false, new PropertyChangedCallback(OnShowDataLabelLinesChanged)));

    private static void OnShowDataLabelLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnShowDataLabelLinesChanged();
    }

    private void OnShowDataLabelLinesChanged()
    {
      // TODO: update the data labels without rebuilding the entire chart.
      Rebuild();
    }

    #endregion // ShowDataLabelLines property

    #region DataLabelOffsetFactor property

    /// <summary>
    /// Gets or sets a value used to determine the distance between a data label and the center of the pie chart.
    /// This is a dependency property.
    /// </summary>
    public double DataLabelOffsetFactor
    {
      get { return (double)GetValue(DataLabelOffsetFactorProperty); }
      set { SetValue(DataLabelOffsetFactorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DataLabelOffsetFactor"/> property.
    /// </summary>
    public static readonly DependencyProperty DataLabelOffsetFactorProperty =
      DependencyProperty.Register("DataLabelOffsetFactor", typeof(double), typeof(PieSeries),
      new PropertyMetadata(0.8, new PropertyChangedCallback(OnDataLabelOffsetFactorChanged)));

    private static void OnDataLabelOffsetFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnDataLabelOffsetFactorChanged();
    }

    private void OnDataLabelOffsetFactorChanged()
    {
      // TODO: update the data labels without rebuilding the entire chart.
      Rebuild();
    }

    #endregion // DataLabelOffsetFactor property

    #region DataLabelStyle property

    /// <summary>
    /// Gets or sets the DataLabelStyle.
    /// This is a dependency property.
    /// </summary>
    public Style DataLabelStyle
    {
      get { return (Style)GetValue(DataLabelStyleProperty); }
      set { SetValue(DataLabelStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DataLabelStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty DataLabelStyleProperty =
      DependencyProperty.Register("DataLabelStyle", typeof(Style), typeof(PieSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnDataLabelStyleChanged)));

    /*private static Style BuildDefaultDataLabelStyle()
    {
      string xaml = "<Style xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:local=\"clr-namespace:Mindscape.WpfElements.Charting;assembly=Mindscape.WpfElements\" TargetType=\"{x:Type local:DataLabel}\">"
          + "<Setter Property=\"Foreground\" Value=\"White\" />"
          + "<Setter Property=\"IsHitTestVisible\" Value=\"False\" />"
          + "<Setter Property=\"Template\">"
          + "  <Setter.Value>"
          + "    <ControlTemplate>"
          + "      <Border Background=\"{Binding Background, RelativeSource={RelativeSource TemplatedParent}}\">"
          + "        <Border Background=\"#55FFFFFF\" Margin=\"1\">"
          + "          <TextBlock Text=\"{Binding DependentData, Converter={StaticResource StringConverter}, RelativeSource={RelativeSource TemplatedParent}}\""
          + "                     Margin=\"2,0,2,0\" Foreground=\"{TemplateBinding Foreground}\" />"
          + "        </Border>"
          + "      </Border>"
          + "    </ControlTemplate>"
          + "  </Setter.Value>"
          + "</Setter>"
          + "</Style>";
      using (XmlReader reader = XmlReader.Create(new StringReader(xaml)))
      {
        Style style = XamlReader.Load(reader) as Style;
        return style;
      }
    }*/

    private static void OnDataLabelStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnDataLabelStyleChanged();
    }

    private void OnDataLabelStyleChanged()
    {
      // TODO: apply new data label style to the data labels.
    }

    #endregion // DataLabelStyle property

    #region StartAngle property

    /// <summary>
    /// Gets or sets the starting angle of the <see cref="PieSeries"/> in degrees.
    /// This is a dependency property.
    /// </summary>
    public double StartAngle
    {
      get { return (double)GetValue(StartAngleProperty); }
      set { SetValue(StartAngleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartAngle"/> property.
    /// </summary>
    public static readonly DependencyProperty StartAngleProperty =
      DependencyProperty.Register("StartAngle", typeof(double), typeof(PieSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnStartAngleChanged)));

    private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnStartAngleChanged();
    }

    private void OnStartAngleChanged()
    {
      Rebuild();
    }

    #endregion // StartAngle property

    #region PieSliceStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> to apply to the <see cref="PieSlice"/> objects.
    /// This is a dependency property.
    /// </summary>
    public Style PieSliceStyle
    {
      get { return (Style)GetValue(PieSliceStyleProperty); }
      set { SetValue(PieSliceStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PieSliceStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty PieSliceStyleProperty =
      DependencyProperty.Register("PieSliceStyle", typeof(Style), typeof(PieSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnPieSliceStyleChanged)));

    private static void OnPieSliceStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnPieSliceStyleChanged();
    }

    private void OnPieSliceStyleChanged()
    {
      Rebuild();
    }

    #endregion // PieSliceStyle property

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
      DependencyProperty.Register("LegendIconTemplate", typeof(DataTemplate), typeof(PieSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnLegendIconTemplateChanged)));

    private static void OnLegendIconTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnLegendIconTemplateChanged();
    }

    private void OnLegendIconTemplateChanged()
    {
      OnLegendItemsChanged();
    }

    #endregion // LegendIconTemplate property

    #region SelectionMode property

    /// <summary>
    /// Gets or sets the SelectionMode.
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
      DependencyProperty.Register("SelectionMode", typeof(DataPointSelectionMode), typeof(PieSeries),
      new PropertyMetadata(DataPointSelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)));

    private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnSelectionModeChanged();
    }

    private void OnSelectionModeChanged()
    {
      foreach (DataPoint dataPoint in SelectedDataPoints)
      {
        dataPoint.SetIsSelected(false);
      }
      SelectedDataPoints.Clear();
      SelectedDataPoint = null;
    }

    #endregion // SelectionMode property

    #region FullCircleTotal property

    /// <summary>
    /// Gets or sets a value that determines if a partial chart or a full chart should be drawn.
    /// If the total sum of data is less than the FullCircleTotal, then this is a partial <see cref="PieSeries"/>.
    /// The default is zero.
    /// This is a dependency property.
    /// </summary>
    public double FullCircleTotal
    {
      get { return (double)GetValue(FullCircleTotalProperty); }
      set { SetValue(FullCircleTotalProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FullCircleTotal"/> property.
    /// </summary>
    public static readonly DependencyProperty FullCircleTotalProperty =
      DependencyProperty.Register("FullCircleTotal", typeof(double), typeof(PieSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnFullCircleTotalChanged)));

    private static void OnFullCircleTotalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PieSeries)d).OnFullCircleTotalChanged();
    }

    private void OnFullCircleTotalChanged()
    {
      Rebuild();
    }

    #endregion // FullCircleTotal property

    private Binding _titleBinding;

    /// <summary>
    /// A binding used to extract the title of a data object.
    /// </summary>
    public Binding TitleBinding
    {
      get { return _titleBinding; }
      set
      {
        _titleBinding = value;
        Rebuild();
        OnLegendItemsChanged();
      }
    }

    private string ApplyTitle(PieSlice slice, int index)
    {
      slice.Title = "Item " + (index + 1);
      if (TitleBinding != null)
      {
        slice.SetBinding(PieSlice.TitleProperty, TitleBinding);
      }
      else if (slice.DataContext is StringDouble)
      {
        slice.SetBinding(PieSlice.TitleProperty, new Binding("String"));
      }
      return slice.Title;
    }

    /// <summary>
    /// Raised when the legend items changed.
    /// </summary>
    public event EventHandler LegendItemsChanged;

    private void OnLegendItemsChanged()
    {
      EventHandler handler = LegendItemsChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    /// <summary>
    /// Gets a list of legend items to be displayed in a legend for this <see cref="PieSeries"/>.
    /// </summary>
    public IEnumerable<LegendItem> LegendItems
    {
      get
      {
        if (ItemsSource != null)
        {
          int index = 0;
          foreach (object o in ItemsSource)
          {
            Brush brush = new SolidColorBrush(Colors.Blue);
            if (Brushes == null || Brushes.Count == 0)
            {
              FillBrushes();
            }
            else
            {
              _brushesLock = true;
            }
            if (Brushes != null && Brushes.Count > 0)
            {
              brush = Brushes[index];
            }
            yield return new LegendItem(ApplyTitle(new PieSlice(o), index), LegendIconTemplate, brush);
            if (Brushes.Count > 0)
            {
              index = ++index % Brushes.Count;
            }
          }
        }
      }
    }

    /// <summary>
    /// Gets the <see cref="Canvas"/> that the pie series is plotted on.
    /// </summary>
    public Canvas Canvas
    {
      get { return _canvas; }
      internal set
      {
        _canvas = value;
      }
    }

    /// <summary>
    /// Gets or sets a binding used to extract a data value from a data object in the <see cref="ItemsSource"/>.
    /// </summary>
    public Binding DataBinding
    {
      get { return _dataBinding; }
      set
      {
        _dataBinding = value;
        //BuildChart();
      }
    }

    /// <summary>
    /// Gets or sets a binding used to extract a radius factor from a data object in the <see cref="ItemsSource"/>.
    /// This can be used to create a pie chart with data pieces of different radius scales.
    /// </summary>
    public Binding RadiusBinding
    {
      get { return _radiusBinding; }
      set
      {
        _radiusBinding = value;
        //BuildChart();
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

    private double GetData(PieSlice o)
    {
      if (o.DataContext is double)
      {
        o.DataValue = (double)o.DataContext;
        return (double)o.DataContext;
      }
      if (DataBinding != null)
      {
        o.SetBinding(PieSlice.DataValueProperty, DataBinding);
      }
      else if (o.DataContext is StringDouble)
      {
        o.SetBinding(PieSlice.DataValueProperty, new Binding("Double"));
      }
      return o.DataValue;
    }

    private double ApplyRadiusBinding(PieSlice o)
    {
      if (o.DataContext is double)
      {
        o.RadiusFactor = 1;
        return 1;
      }
      if (RadiusBinding != null)
      {
        o.SetBinding(PieSlice.RadiusFactorProperty, RadiusBinding);
        //return Math.Max(0, Math.Min(o.Size, 1));
      }
      else
      {
        o.RadiusFactor = 1;
      }
      return o.RadiusFactor;
    }

    private void ApplyLabelBinding(PieSlice slice)
    {
      if (LabelBinding != null)
      {
        slice.SetBinding(DataPoint.LabelContentProperty, LabelBinding);
      }
    }

    private void ApplyToolTipBinding(PieSlice dataPoint)
    {
      if (ToolTipBinding != null)
      {
        dataPoint.SetBinding(PieSlice.ToolTipProperty, ToolTipBinding);
      }
    }

    private PieSlice GetPieSlice(object o, int index)
    {
      PieSlice slice;
      _dataPointMap.TryGetValue(new IndexData(index, o), out slice);
      if (slice == null)
      {
        slice = new PieSlice(o, DoughnutScale);
      }
      slice.DoughnutScale = DoughnutScale;
      slice.DataValueChanged -= new EventHandler(PieSlice_DataValueChanged);
      //slice.IsSelectedChanged -= new EventHandler(PieSlice_IsSelectedChanged);
      slice.IsSelectedChangeRequested -= new EventHandler<DataPoint.SelectionChangeRequestArgs>(PieSlice_IsSelectedChangeRequested);
      slice.ExplodedDistanceChanged -= new EventHandler(PieSlice_ExplodedDistanceChanged);
      return slice;
    }

    internal void BuildChart()
    {
      if (_canvas != null && ItemsSource != null)
      {
        double sum = 0;
        foreach (object o in ItemsSource)
        {
          double data = GetData(new PieSlice(o));
          sum += data;
        }
        double startAngle = Math.PI / 180.0 * StartAngle;
        double sweepAngle = 0;

        int brushIndex = 0;
        int index = 0;
        foreach (object o in ItemsSource)
        {
          PieSlice slice = GetPieSlice(o, index);
          //PieSlice slice = new PieSlice(o, DoughnutScale);
          double data = GetData(slice);
          ApplyRadiusBinding(slice);
          ApplyTitle(slice, index); // this sets the PieSlice.Title property
          ApplyLabelBinding(slice);
          ApplyToolTipBinding(slice);
          slice.PieSeries = this;
          // TODO: may want to unhook this when a pie slice is no longer needed.
          //slice.IsSelectedChanged += new EventHandler(PieSlice_IsSelectedChanged);
          slice.IsSelectedChangeRequested += new EventHandler<DataPoint.SelectionChangeRequestArgs>(PieSlice_IsSelectedChangeRequested);
          slice.ExplodedDistanceChanged += new EventHandler(PieSlice_ExplodedDistanceChanged);
          slice.DataValueChanged += new EventHandler(PieSlice_DataValueChanged);
          slice.Percentage = data / sum * 100;
          _dataPointMap[new IndexData(index, o)] = slice;

          Brush brush = new SolidColorBrush(Colors.Blue);
          if (Brushes.Count == 0)
          {
            FillBrushes();
          }
          else
          {
            _brushesLock = true;
          }
          if (Brushes.Count != 0)
          {
            brush = Brushes[brushIndex];
            brushIndex = ++brushIndex % Brushes.Count;
          }

          if (sum < FullCircleTotal)
          {
            startAngle += sweepAngle;
            sweepAngle = 2 * Math.PI * (data / FullCircleTotal);
          }
          else
          {
            startAngle += sweepAngle;
            sweepAngle = 2 * Math.PI * data / sum;
          }
          if (Double.IsNaN(sweepAngle))
          {
            sweepAngle = 0;
          }
          sweepAngle = Math.Min(sweepAngle, Math.PI / 180.0 * 359.99);
          double overlappingSweepAngle = Math.Min(sweepAngle + 0.01, Math.PI / 180.0 * 359.99);
          double dx = slice.ExplodedDistance * Math.Cos(startAngle + sweepAngle / 2);
          double dy = slice.ExplodedDistance * Math.Sin(startAngle + sweepAngle / 2);
          BuildArc(slice, brush, startAngle, startAngle + overlappingSweepAngle, dx, dy);

          // Data Label:
          AddDataLabel(slice, brush, startAngle, sweepAngle);

          index++;
        }
      }
    }

    private void AddDataLabel(PieSlice slice, Brush brush, double startAngle, double sweepAngle)
    {
      if (ShowDataLabels)
      {
        DataLabel label = new DataLabel(slice, slice.DataValue);
        label.Background = brush;
        if (DataLabelStyle != null)
        {
          label.Style = DataLabelStyle;
        }
        label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        double labelWidth = label.DesiredSize.Width;
        double labelHeight = label.DesiredSize.Height;
        double labelAngle = startAngle + sweepAngle / 2;
        double xc = _canvas.ActualWidth / 2;
        double yc = _canvas.ActualHeight / 2;

        double labelOffsetFactor = DataLabelOffsetFactor;
        double horizontalLabelOffset = 0;
        double doughnutRadius = 0.8 * Math.Min(yc, xc) * DoughnutScale;
        double maxRadius = 0.8 * Math.Min(yc, xc) * MainRadiusPercentage;
        double radiusDiff = maxRadius - doughnutRadius;
        double labelRadius = doughnutRadius * DataLabelOffsetFactor + radiusDiff;
        Point labelPoint = new Point(xc + labelRadius * Math.Cos(labelAngle), yc + labelRadius * Math.Sin(labelAngle));
        if (ShowDataLabelLines)
        {
          labelOffsetFactor = 1.2;
          labelRadius = 0.8 * Math.Min(yc, xc) * labelOffsetFactor;
          Point intersection = new Point(xc + labelRadius * Math.Cos(labelAngle), yc + labelRadius * Math.Sin(labelAngle));

          double edgeRadius = 0.8 * Math.Min(yc, xc);
          Point edgePoint = new Point(xc + edgeRadius * Math.Cos(labelAngle), yc + edgeRadius * Math.Sin(labelAngle));

          horizontalLabelOffset = 40 + labelWidth / 2.0;
          if (intersection.X < edgePoint.X)
          {
            horizontalLabelOffset = -40 - labelWidth / 2.0;
          }
          labelPoint = new Point(intersection.X + horizontalLabelOffset, intersection.Y);
          labelPoint = AdjustLabelPosition(labelPoint, labelWidth, labelHeight);

          double deltaY = intersection.Y - edgePoint.Y;
          double deltaX = intersection.X - edgePoint.X;
          if (deltaY == 0)
          {
            intersection = edgePoint;
          }
          else if (deltaX == 0)
          {
            intersection = new Point(edgePoint.X, labelPoint.Y);
          }
          else
          {
            double gradient = deltaY / deltaX;
            double y = labelPoint.Y - edgePoint.Y;
            double x = y / gradient;
            intersection = new Point(edgePoint.X + x, labelPoint.Y);
          }

          Line line1 = new Line();
          line1.StrokeThickness = 1;
          line1.Stroke = brush;
          line1.X1 = edgePoint.X;
          line1.Y1 = edgePoint.Y;
          line1.X2 = intersection.X;
          line1.Y2 = intersection.Y;
          Canvas.SetZIndex(line1, -100);
          Canvas.Children.Add(line1);
          Line line2 = new Line();
          line2.StrokeThickness = 1;
          line2.Stroke = brush;
          line2.X1 = intersection.X;
          line2.Y1 = intersection.Y;
          line2.X2 = labelPoint.X;
          line2.Y2 = labelPoint.Y;
          Canvas.Children.Add(line2);
        }
        Canvas.SetLeft(label, Math.Round(labelPoint.X - labelWidth / 2));
        Canvas.SetTop(label, Math.Round(labelPoint.Y - labelHeight / 2));
        Canvas.SetZIndex(label, 200);

        Canvas.Children.Add(label);
      }
    }

    private void PieSlice_IsSelectedChangeRequested(object sender, DataPoint.SelectionChangeRequestArgs e)
    {
      PieSlice dataPoint = sender as PieSlice;
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
          foreach (PieSlice dp in _dataPointMap.Values)
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

    private void PieSlice_DataValueChanged(object sender, EventArgs e)
    {
      Rebuild();
    }

    private Point AdjustLabelPosition(Point labelPoint, double labelWidth, double labelHeight)
    {
      double buffer = 3;
      if (labelPoint.Y - labelHeight / 2 - buffer < 0)
      {
        labelPoint.Y = buffer + labelHeight / 2;
      }
      if (labelPoint.Y + labelHeight / 2 + buffer > Canvas.ActualHeight)
      {
        labelPoint.Y = Canvas.ActualHeight - labelHeight / 2 - buffer;
      }
      if (labelPoint.X - labelWidth / 2 - buffer < 0)
      {
        labelPoint.X = labelWidth / 2 + buffer;
      }
      if (labelPoint.X + labelWidth / 2 + buffer > Canvas.ActualWidth)
      {
        labelPoint.X = Canvas.ActualWidth - labelWidth / 2 - buffer;
      }
      return labelPoint;
    }

    private void BuildArc(PieSlice slice, Brush brush, double startAngle, double endAngle, double dx, double dy)
    {
      double r;
      Point centerPosition;
      PathGeometry pg = BuildArcGeometry(startAngle, endAngle, dx, dy, slice.RadiusFactor, out r, out centerPosition);

      slice.Radius = r;
      slice.Diameter = 2 * r;
      slice.PathData = pg;
      slice.StartAngle = startAngle;
      slice.EndAngle = endAngle;
      slice.Width = Canvas.ActualWidth;
      slice.Height = Canvas.ActualHeight;
      slice.Background = brush;
      slice.CenterPoint = centerPosition;
      if (PieSliceStyle != null)
      {
        slice.Style = PieSliceStyle;
      }
      if (!_canvas.Children.Contains(slice))
      {
        _canvas.Children.Add(slice);
      }
    }

    private void PieSlice_ExplodedDistanceChanged(object sender, EventArgs e)
    {
      PieSlice slice = sender as PieSlice;

      double startAngle = slice.StartAngle;
      double endAngle = slice.EndAngle;
      double size = slice.RadiusFactor;
      double sweepAngle = endAngle - startAngle;
      double dx = slice.ExplodedDistance * Math.Cos(startAngle + sweepAngle / 2);
      double dy = slice.ExplodedDistance * Math.Sin(startAngle + sweepAngle / 2);

      double r;
      Point centerPoint;
      PathGeometry geometry = BuildArcGeometry(startAngle, endAngle, dx, dy, size, out r, out centerPoint);

      slice.PathData = geometry;
      slice.CenterPoint = centerPoint;
    }

    private PathGeometry BuildArcGeometry(double startAngle, double endAngle, double dx, double dy, double size, out double r, out Point centerPosition)
    {
      bool isLarge = (endAngle - startAngle) > Math.PI;
      PathGeometry pg = new PathGeometry();
      PathFigure pf = new PathFigure();
      LineSegment ls1 = new LineSegment();
      LineSegment ls2 = new LineSegment();
      ArcSegment arc = new ArcSegment();
      ArcSegment holeArc = new ArcSegment();
      double xc = _canvas.ActualWidth / 2 + dx;
      double yc = _canvas.ActualHeight / 2 + dy;
      double maxRadius = (PieChart != null ? PieChart.RadiusFactor : 0.8) * Math.Min(_canvas.ActualHeight / 2, _canvas.ActualWidth / 2);
      r = maxRadius * MainRadiusPercentage * size;

      Point startPoint = new Point(xc, yc);
      centerPosition = new Point(xc, yc);

      if (DoughnutScale != 1)
      {
        double holeRadius = maxRadius * (1 * MainRadiusPercentage - DoughnutScale);
        startPoint = new Point(xc + holeRadius * Math.Cos(startAngle), yc + holeRadius * Math.Sin(startAngle));
      }

      pf.IsClosed = true;
      if (ItemsSource.Count > 1 || IsSingleDataLineVisible)
      {
        pf.StartPoint = startPoint;
        pf.Segments.Add(ls1);
        pf.Segments.Add(arc);
        pf.Segments.Add(ls2);
        if (DoughnutScale != 1)
        {
          pf.Segments.Add(holeArc);
        }
      }
      else
      {
        pf.StartPoint = new Point(xc + r * Math.Cos(startAngle), yc + r * Math.Sin(startAngle));
        pf.Segments.Add(arc);
      }
      pg.Figures.Add(pf);

      ls1.Point = new Point(xc + r * Math.Cos(startAngle), yc + r * Math.Sin(startAngle));
      arc.Point = new Point(xc + r * Math.Cos(endAngle), yc + r * Math.Sin(endAngle));

      if (DoughnutScale != 1)
      {
        double holeRadius = maxRadius * (1 * MainRadiusPercentage - DoughnutScale);
        Point holePoint2 = new Point(xc + holeRadius * Math.Cos(endAngle), yc + holeRadius * Math.Sin(endAngle));
        centerPosition = new Point(xc + holeRadius * Math.Cos((endAngle - startAngle) / 2), yc + holeRadius * Math.Sin((endAngle - startAngle) / 2));

        ls2.Point = holePoint2;
        holeArc.Point = startPoint;

        holeArc.SweepDirection = SweepDirection.Counterclockwise;
        holeArc.Size = new Size(holeRadius, holeRadius);
        holeArc.IsLargeArc = isLarge;
      }
      else
      {
        ls2.Point = new Point(xc + r * Math.Cos(endAngle), yc + r * Math.Sin(endAngle));
      }

      arc.SweepDirection = SweepDirection.Clockwise;
      arc.Size = new Size(r, r);
      arc.IsLargeArc = isLarge;

      return pg;
    }

    private PieSlice _selectedSlice;

    /// <summary>
    /// Gets the currently selected <see cref="CartesianDataPoint"/>.
    /// This will be null if none of the data points in the <see cref="DataSeries"/> are selected.
    /// </summary>
    public PieSlice SelectedDataPoint
    {
      get { return _selectedSlice; }
      private set
      {
        if (_selectedSlice != value)
        {
          _selectedSlice = value;
          OnSelectedDataPointChanged();
        }
      }
    }

    private ObservableCollection<PieSlice> _selectedDataPoints = new ObservableCollection<PieSlice>();

    // TODO: make this public and implement logic to make sure the selection mode is respected.
    private ObservableCollection<PieSlice> SelectedDataPoints
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

    // This is internal for now because we may want to include more event data such as the previous selected data point.
    //internal event EventHandler SelectedDataPointChanged;

    /// <summary>
    /// Raised when the selected data point changes.
    /// </summary>
    public event EventHandler<SelectedDataPointChangedEventArgs> SelectedDataPointChanged;

    internal void DeselectAll()
    {
      foreach (PieSlice dataPoint in _dataPointMap.Values)
      {
        dataPoint.SetIsSelected(false);
      }
      SelectedDataPoint = null;
    }

    /*private void PieSlice_IsSelectedChanged(object sender, EventArgs e)
    {
      if (_selectedSlice != null)
      {
        _selectedSlice.IsSelected = false;
      }
      PieSlice slice = sender as PieSlice;
      if (slice.IsSelected)
      {
        _selectedSlice = slice;
      }
      else
      {
        _selectedSlice = null;
      }
    }*/
  }
}

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
using System.ComponentModel;
using System.Collections.Generic;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a chart axis used by a <see cref="PolarChart"/>.
  /// </summary>
  public abstract class PolarAxisBase : Control
  {
    private const string LabelsHostPartName = "PART_LabelsHost";

    private ObservableCollection<double> _majorTickMarks = new ObservableCollection<double>();
    private ObservableCollection<ContentControl> _labels = new ObservableCollection<ContentControl>();

    private Dictionary<double, object> _labelMap;
    private Dictionary<object, double> _dataMap = new Dictionary<object, double>();

    private double _spacing;

    private bool _canRaiseAxisUpdated = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarAxisBase"/> class.
    /// </summary>
    public PolarAxisBase()
    {
      SizeChanged += new SizeChangedEventHandler(PolarAxisBase_SizeChanged);
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      ItemsControl labelsHost = GetTemplateChild(LabelsHostPartName) as ItemsControl;
      if (labelsHost != null)
      {
        labelsHost.ItemsSource = _labels;
      }
    }

    private void PolarAxisBase_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateAxis();
    }

    // Lets internal stuff know if this axis was automatically created rather than manually by the programmer.
    internal bool IsAuto { get; set; }

    internal bool CanRaiseAxisUpdated
    {
      set { _canRaiseAxisUpdated = value; }
    }

    #region Minimum property

    /// <summary>
    /// Gets or sets the minimum axis value.
    /// This is a dependency property.
    /// </summary>
    public double Minimum
    {
      get { return (double)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
      DependencyProperty.Register("Minimum", typeof(double), typeof(PolarAxisBase),
      new PropertyMetadata(new PropertyChangedCallback(OnMinimumChanged)));

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarAxisBase)d).OnMinimumChanged();
    }

    private void OnMinimumChanged()
    {
      if (Minimum > Maximum)
      {
        Maximum = Minimum;
      }
      UpdateSpacing();
      UpdateAxis();
    }
    
    #endregion // Minimum property

    #region Maximum property

    /// <summary>
    /// Gets or sets the maximum axis value.
    /// This is a dependency property.
    /// </summary>
    public double Maximum
    {
      get { return (double)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
      DependencyProperty.Register("Maximum", typeof(double), typeof(PolarAxisBase),
      new PropertyMetadata(new PropertyChangedCallback(OnMaximumChanged)));

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarAxisBase)d).OnMaximumChanged();
    }

    private void OnMaximumChanged()
    {
      if (Maximum < Minimum)
      {
        Minimum = Maximum;
      }
      UpdateSpacing();
      UpdateAxis();
    }

    #endregion // Maximum property

    #region ValueConverter property

    /// <summary>
    /// Gets or sets the <see cref="IAxisValueConverter"/> used for converting between axis values and objects.
    /// This is a dependency property.
    /// </summary>
    public IAxisValueConverter ValueConverter
    {
      get { return (IAxisValueConverter)GetValue(ValueConverterProperty); }
      set { SetValue(ValueConverterProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ValueConverter"/> property.
    /// </summary>
    public static readonly DependencyProperty ValueConverterProperty =
      DependencyProperty.Register("ValueConverter", typeof(IAxisValueConverter), typeof(PolarAxisBase),
      new PropertyMetadata(new PropertyChangedCallback(OnValueConverterChanged)));

    private static void OnValueConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarAxisBase)d).OnValueConverterChanged();
    }

    private void OnValueConverterChanged()
    {
    }

    #endregion // ValueConverter property

    #region LabelFormat property

    /// <summary>
    /// Gets or sets the LabelFormat.
    /// This is a dependency property.
    /// </summary>
    public string LabelFormat
    {
      get { return (string)GetValue(LabelFormatProperty); }
      set { SetValue(LabelFormatProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelFormat"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelFormatProperty =
      DependencyProperty.Register("LabelFormat", typeof(string), typeof(PolarAxisBase),
      new PropertyMetadata("{0:0.###}", new PropertyChangedCallback(OnLabelFormatChanged)));

    private static void OnLabelFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarAxisBase)d).OnLabelFormatChanged();
    }

    private void OnLabelFormatChanged()
    {
    }

    #endregion // LabelFormat property

    #region LabelTemplate property

    /// <summary>
    /// Gets or sets the LabelTemplate.
    /// This is a dependency property.
    /// </summary>
    public DataTemplate LabelTemplate
    {
      get { return (DataTemplate)GetValue(LabelTemplateProperty); }
      set { SetValue(LabelTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelTemplateProperty =
      DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(PolarAxisBase),
      new PropertyMetadata(new PropertyChangedCallback(OnLabelTemplateChanged)));

    private static void OnLabelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarAxisBase)d).OnLabelTemplateChanged();
    }

    private void OnLabelTemplateChanged()
    {
      foreach (ContentControl label in _labels)
      {
        label.ContentTemplate = LabelTemplate;
      }
    }

    #endregion // LabelTemplate property

    #region MajorTickSpacing property

    /// <summary>
    /// Gets or sets the MajorTickSpacing.
    /// This is a dependency property.
    /// </summary>
    public double MajorTickSpacing
    {
      get { return (double)GetValue(MajorTickSpacingProperty); }
      set { SetValue(MajorTickSpacingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MajorTickSpacing"/> property.
    /// </summary>
    public static readonly DependencyProperty MajorTickSpacingProperty =
      DependencyProperty.Register("MajorTickSpacing", typeof(double), typeof(PolarAxisBase),
      new PropertyMetadata(0.0, new PropertyChangedCallback(OnMajorTickSpacingChanged)));

    private static void OnMajorTickSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarAxisBase)d).OnMajorTickSpacingChanged();
    }

    private void OnMajorTickSpacingChanged()
    {
      UpdateSpacing();
    }

    #endregion // MajorTickSpacing property

    internal Dictionary<double, object> LabelMap
    {
      get { return _labelMap; }
      private set
      {
        _labelMap = value;
        UpdateAxis();
      }
    }

    /// <summary>
    /// Calculates the logical data position of the given object against this polar axis.
    /// </summary>
    /// <param name="o">The object to calculate the position of.</param>
    /// <returns>The logical position of the given object against this polar axis.</returns>
    public double GetLogicalPosition(object o)
    {
      double position = 0;
      if (_dataMap.ContainsKey(o))
      {
        position = _dataMap[o];
      }
      else if (ValueConverter != null)
      {
        position = ValueConverter.GetAxisPlotPosition(o);
      }
      else if (NumericalUtils.IsPrimitiveNumerical(o))
      {
        position = NumericalUtils.ConvertToDouble(o) ?? 0;
      }
      else
      {
        position = _dataMap.Count;
        _dataMap[o] = position;
        if (LabelMap == null)
        {
          LabelMap = new Dictionary<double, object>();
        }
        int count = LabelMap.Count;
        LabelMap[position] = o;
        if (LabelMap.Count != count)
        {
          UpdateAxis();
        }
      }
      return position;
    }

    /// <summary>
    /// Updates the axis visual.
    /// </summary>
    protected void UpdateAxis()
    {
      UpdateAxisCore();
      OnAxisUpdated(); // For updateing chart grids and possibly some custom elements too.
      OnAxisUpdatedInternal(); // For internally re-building the chart if necessary.
    }

    /// <summary>
    /// Raised when the axis is updated.
    /// </summary>
    public event EventHandler AxisUpdated;

    private void OnAxisUpdated()
    {
      EventHandler handler = AxisUpdated;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    internal event EventHandler AxisUpdatedInternal;

    private void OnAxisUpdatedInternal()
    {
      if (_canRaiseAxisUpdated)
      {
        EventHandler handler = AxisUpdatedInternal;
        if (handler != null)
        {
          handler(this, new EventArgs());
        }
      }
    }

    /// <summary>
    /// When overriden, this method manages the creation and placement of axis labels.
    /// </summary>
    protected abstract void UpdateAxisCore();

    internal void AddAxisLabel(int labelCount, object labelContent, string formattedLabelContent, double x, double y)
    {
      if (labelCount < _labels.Count)
      {
        UpdateAxisLabel(_labels[labelCount], labelContent, formattedLabelContent, x, y);
      }
      else
      {
        ContentControl label = BuildAxisLabel(labelContent, formattedLabelContent, x, y);
        _labels.Add(label);
      }
    }

    private void UpdateAxisLabel(ContentControl label, object labelContent, string formattedLabelContent, double x, double y)
    {
      label.Content = new AxisLabel(labelContent, formattedLabelContent);
      label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      TranslateTransform translation = label.RenderTransform as TranslateTransform;
      translation.X = x - (label.DesiredSize.Width / 2.0);
      translation.Y = y - (label.DesiredSize.Height / 2.0);
    }

    private ContentControl BuildAxisLabel(object labelContent, string formattedLabelContent, double x, double y)
    {
      ContentControl label = new ContentControl();
      label.ContentTemplate = LabelTemplate;
      label.Foreground = Foreground; // TODO: Is this needed? Could this be messing up the label template foreground?
      label.Content = new AxisLabel(labelContent, formattedLabelContent);
      label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      label.RenderTransform = new TranslateTransform() { X = x - (label.DesiredSize.Width / 2.0), Y = y - (label.DesiredSize.Height / 2.0) };
      label.HorizontalAlignment = HorizontalAlignment.Left;
      label.VerticalAlignment = VerticalAlignment.Top;
      return label;
    }

    internal void RemoveLeftOverAxisLabels(int labelCount)
    {
      while (_labels.Count > labelCount)
      {
        _labels.RemoveAt(_labels.Count - 1);
      }
    }

    internal object GetLabel(double labelPosition)
    {
      object labelContent = labelPosition;
      if (ValueConverter != null)
      {
        labelContent = ValueConverter.GetDataObjectAt(labelPosition);
      }
      else if (LabelMap != null && LabelMap.Count > 0)
      {
        LabelMap.TryGetValue((int)labelPosition, out labelContent);
      }
      if (labelContent == null)
      {
        labelContent = labelPosition;
      }
      return labelContent;
    }

    internal string GetLabelString(string labelFormat, object labelObject)
    {
      string labelString = null;
      if (labelFormat != null)
      {
        try
        {
          labelString = String.Format(labelFormat, labelObject);
        }
        catch (FormatException)
        {
          labelString = labelObject.ToString();
        }
      }
      else
      {
        labelString = labelObject.ToString();
      }
      return labelString;
    }

    internal double Spacing
    {
      get { return _spacing; }
      set { _spacing = value; }
    }

    internal abstract void UpdateSpacing();

    /*/// <summary>
    /// Gets the collection of the generated axis labels to place along the <see cref="ThetaAxis"/>.
    /// </summary>
    public ReadOnlyCollection<AxisLabel> Labels
    {
      get { return new ReadOnlyCollection<AxisLabel>(_labels); }
    }*/

    internal ObservableCollection<double> MajorTickMarks
    {
      get { return _majorTickMarks; }
    }
  }
}

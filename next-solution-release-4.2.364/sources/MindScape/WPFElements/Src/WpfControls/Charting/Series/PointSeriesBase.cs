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
using System.Diagnostics;
using System.Collections.Generic;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A base class for data series which plot data using points, such as line series or
  /// scatter graphs.
  /// </summary>
  public abstract class PointSeriesBase : DataSeries
  {
    #region SymbolStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to the data points.
    /// This is a dependency property.
    /// </summary>
    public Style SymbolStyle
    {
      get { return (Style)GetValue(SymbolStyleProperty); }
      set { SetValue(SymbolStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SymbolStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty SymbolStyleProperty =
      DependencyProperty.Register("SymbolStyle", typeof(Style), typeof(PointSeriesBase),
      new PropertyMetadata(new PropertyChangedCallback(OnSymbolStyleChanged)));

    private static void OnSymbolStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PointSeriesBase)d).OnSymbolStyleChanged();
    }

    private void OnSymbolStyleChanged()
    {
      // TODO: a better way to update the style is to somehow iterate through the symbols that have been created by this series. Keep in mind the SeriesBrush property.
      RequestRebuild();
    }

    #endregion // SymbolStyle property

    #region SymbolSize Property

    /// <summary>
    /// Gets or sets the size of the symbols rendered by this data series.
    /// This property affects both the width and height of the symbols.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SymbolSizeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double SymbolSize
    {
      get { return (double)GetValue(SymbolSizeProperty); }
      set { SetValue(SymbolSizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SymbolSize"/> property.
    /// </summary>
    public static readonly DependencyProperty SymbolSizeProperty =
      DependencyProperty.Register("SymbolSize", typeof(double), typeof(PointSeriesBase),
      new FrameworkPropertyMetadata(OnSymbolSizeChanged));

    private static void OnSymbolSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PointSeriesBase)d).OnSymbolSizeChanged();
    }

    private void OnSymbolSizeChanged()
    {
      RequestRebuild();
    }

    #endregion // SymbolSize Property

    #region ShowDataLabels property

    /// <summary>
    /// Gets or sets whether or not to display the data labels.
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
      DependencyProperty.Register("ShowDataLabels", typeof(bool), typeof(PointSeriesBase),
      new PropertyMetadata(false, new PropertyChangedCallback(OnShowDataLabelsChanged)));

    private static void OnShowDataLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PointSeriesBase)d).OnShowDataLabelsChanged();
    }

    private void OnShowDataLabelsChanged()
    {
      // TODO: possibly find a way to update the data labels without updating the whole chart
      RequestRebuild();
    }

    #endregion // ShowDataLabels property

    #region ShowDataLabelLines property

    /// <summary>
    /// Gets or sets whether or not to display the data label lines.
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
      DependencyProperty.Register("ShowDataLabelLines", typeof(bool), typeof(PointSeriesBase),
      new PropertyMetadata(false, new PropertyChangedCallback(OnShowDataLabelLinesChanged)));

    private static void OnShowDataLabelLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PointSeriesBase)d).OnShowDataLabelLinesChanged();
    }

    private void OnShowDataLabelLinesChanged()
    {
      // TODO: possibly find a way to update the data labels without updating the whole chart
      RequestRebuild();
    }

    #endregion // ShowDataLabelLines property

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
      DependencyProperty.Register("DataLabelStyle", typeof(Style), typeof(PointSeriesBase),
      new PropertyMetadata(new PropertyChangedCallback(OnDataLabelStyleChanged)));

    private static void OnDataLabelStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PointSeriesBase)d).OnDataLabelStyleChanged();
    }

    private void OnDataLabelStyleChanged()
    {
      // TODO: apply the new style to all the data labels.
    }

    #endregion // DataLabelStyle property

    /// <summary>
    /// Returns the constrained position of a data label.
    /// </summary>
    /// <param name="left">The desired horizontal position of the data label.</param>
    /// <param name="top">The desired vertical position of the data label.</param>
    /// <param name="labelWidth">The width of the data label.</param>
    /// <param name="labelHeight">The height of the data label.</param>
    /// <returns>The position of the data label constrained by the bounds of the chart canvas.</returns>
    protected Point PerformLabelBoundsCorrection(double left, double top, double labelWidth, double labelHeight)
    {
      double x = Math.Max(left, 3);
      x = Math.Min(x, Canvas.ActualWidth - labelWidth - 3);
      double y = Math.Max(top, 3);
      y = Math.Min(y, Canvas.ActualHeight - labelHeight - 3);
      return new Point(x, y);
    }

    /// <summary>
    /// Returns the width of a data label for the given data object.
    /// </summary>
    /// <param name="data">The data object.</param>
    /// <returns>The width of a data label displaying the given data object.</returns>
    protected double GetWidth(object data)
    {
      TextBlock block = new TextBlock();
      block.Text = String.Format("{0:0.0}", data);
      StackPanel panel = new StackPanel();
      panel.Children.Add(block);
      panel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      return panel.DesiredSize.Width + 8;
    }

    /// <summary>
    /// Returns the height of a data label for the given data object.
    /// </summary>
    /// <param name="data">The data object.</param>
    /// <returns>The width of a data label displaying the given data object.</returns>
    protected double GetHeight(object data)
    {
      TextBlock block = new TextBlock();
      block.Text = String.Format("{0:0.0}", data);
      StackPanel panel = new StackPanel();
      panel.Children.Add(block);
      panel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      return panel.DesiredSize.Height + 4;
    }

    /// <summary>
    /// Gets the <see cref="ChartSymbol"/> for the given data object.
    /// </summary>
    /// <param name="o">The data object that the <see cref="ChartSymbol"/> displays.</param>
    /// <param name="index">The index of the data.</param>
    /// <param name="point">The logical plot position of the data.</param>
    /// <returns>The <see cref="ChartSymbol"/> to display the given data object.</returns>
    protected ChartSymbol GetChartSymbol(object o, int index, out Point point)
    {
      ChartSymbol symbol = GetDataPoint(o, index) as ChartSymbol;
      if (o == null)
      {
        point = new Point(Double.NaN, Double.NaN);
      }
      else if (symbol == null)
      {
        symbol = new ChartSymbol(o);
        point = GetPoint(symbol, index);
      }
      else
      {
        point = symbol.LogicalPoint;
      }
      if (symbol != null && (symbol.YObject == null || symbol.XObject == null))
      {
        symbol = null;
      }
      if (Double.IsNaN(point.X) || Double.IsNaN(point.Y))
      {
        symbol = null;
      }
      return symbol;
    }

    /// <summary>
    /// Sets the style and position of the given <see cref="ChartSymbol"/> and adds it to the canvas.
    /// </summary>
    /// <param name="symbol">The <see cref="ChartSymbol"/> to setup.</param>
    /// <param name="normalPoint">The physical position of the <see cref="ChartSymbol"/> on the chating canvas.</param>
    internal void SetupChartSymbol(ChartSymbol symbol, Point normalPoint)
    {
      if (SymbolStyle != null && symbol != null)
      {
        symbol.Style = SymbolStyle;
        if (symbol.Background == null && SeriesBrush != null)
        {
          symbol.Background = SeriesBrush;
        }
        if (symbol.BorderBrush == null && SeriesBrush != null)
        {
          symbol.BorderBrush = SeriesBrush;
        }
        if (SymbolSize > 0)
        {
          symbol.Width = SymbolSize;
          symbol.Height = SymbolSize;
          double halfSize = SymbolSize / 2.0;
          symbol.Margin = new Thickness(-halfSize, -halfSize, halfSize, halfSize);
        }
        Canvas.SetLeft(symbol, Math.Round(normalPoint.X));
        Canvas.SetTop(symbol, Math.Round(normalPoint.Y));
        if (ForegroundCanvas != null && !Chart.IsChartClipped)
        {
          if (normalPoint.X > -2 && normalPoint.X <= ForegroundCanvas.ActualWidth + 2 && normalPoint.Y > -2 && normalPoint.Y <= ForegroundCanvas.ActualHeight + 2)
          {
            ForegroundCanvas.Children.Add(symbol);
          }
        }
        else
        {
          Canvas.Children.Add(symbol);
        }
      }
    }

    /// <summary>
    /// Adds a data label to the chart canvas.
    /// </summary>
    /// <param name="index">The index of the data to create a label for.</param>
    /// <param name="brush">The <see cref="Brush"/> used to color the data label.</param>
    internal void AddDataLabel(int index, Brush brush)
    {
      if (ShowDataLabels)
      {
        Point currentPoint = GetPoint(index);

        Point nextPoint = currentPoint;
        if (index < ItemsSource.Count - 1)
        {
          nextPoint = GetPoint(index + 1);
        }

        AddDataLabel(index, currentPoint, nextPoint, brush);
      }
    }

    /// <summary>
    /// Adds a data label to the chart canvas.
    /// </summary>
    /// <param name="index">The index of the data to create a label for.</param>
    /// <param name="currentPoint">The current logical data position.</param>
    /// <param name="nextPoint">The next logical data position.</param>
    /// <param name="brush">The <see cref="Brush"/> used to color the data label.</param>
    internal void AddDataLabel(int index, Point currentPoint, Point nextPoint, Brush brush)
    {
      if (ShowDataLabels)
      {
        if (XAxis == null || YAxis == null || currentPoint.X < XAxis.ActualMinimumValue || currentPoint.X > XAxis.ActualMaximumValue || currentPoint.Y < YAxis.ActualMinimumValue || currentPoint.Y > YAxis.ActualMaximumValue)
        {
          return;
        }
        object data = ItemsSource[index];
        Point normalPoint = ConvertLogicalToPhysicalPoint(currentPoint);
        Point nextNormalPoint = ConvertLogicalToPhysicalPoint(nextPoint);

        CartesianDataPoint dataPoint = GetDataPoint(data, -1);
        if (dataPoint == null)
        {
          dataPoint = new CartesianDataPoint();
          dataPoint.DataContext = data;
          GetPoint(dataPoint, index);
        }
        object dependentData = dataPoint.YObject;
        DataLabel label = new DataLabel(dataPoint, dependentData);
        if (DataLabelStyle != null)
        {
          label.Style = DataLabelStyle;
        }
        label.Background = brush;
        double labelWidth = GetWidth(label.DependentData);
        double labelHeight = GetHeight(label.DependentData);

        double labelLeft = normalPoint.X + 10;
        double labelTop = normalPoint.Y + 10;

        if (normalPoint.Y < nextNormalPoint.Y)
        {
          labelTop = normalPoint.Y - 10 - labelHeight;
        }

        Point p = PerformLabelBoundsCorrection(labelLeft, labelTop, labelWidth, labelHeight);
        labelLeft = (int)p.X;
        labelTop = (int)p.Y;
        Canvas.SetLeft(label, labelLeft);
        Canvas.SetTop(label, labelTop);
        Canvas.SetZIndex(label, 200);

        if (ShowDataLabelLines)
        {
          Line line = new Line() { X1 = normalPoint.X, Y1 = normalPoint.Y };
          line.Stroke = brush;
          line.StrokeThickness = 1; // TODO: provide a data label line style

          Point cornerPoint = GetClosestCornerPoint(normalPoint, labelLeft, labelTop, labelWidth, labelHeight);

          line.X2 = cornerPoint.X;
          line.Y2 = cornerPoint.Y;
          Canvas.SetZIndex(line, -1);
          Canvas.Children.Add(line);
        }

        Canvas.Children.Add(label);
      }
    }

    /*internal override void FinishPlottingData()
    {
      int labelIndex = 0;
      foreach (CartesianDataPoint dataPoint in DataPoints)
      {
        if (dataPoint != null)
        {
          Point currentPoint = dataPoint.LogicalPoint;
          if (XAxis == null || YAxis == null || currentPoint.X < XAxis.ActualMinimum || currentPoint.X > XAxis.ActualMaximum || currentPoint.Y < YAxis.ActualMinimum || currentPoint.Y > YAxis.ActualMaximum)
          {
          }
          else
          {
            AddDataLabel(dataPoint, labelIndex, SeriesBrush);
            labelIndex++;
          }
        }
      }
      while (_dataLabels.Count > labelIndex)
      {
        _dataLabels[_dataLabels.Count - 1].SizeChanged -= new SizeChangedEventHandler(Label_SizeChanged);
        _dataLabels.RemoveAt(_dataLabels.Count - 1);
      }
    }*/

    private readonly IList<DataLabel> _dataLabels = new List<DataLabel>();

    internal void AddDataLabel(CartesianDataPoint dataPoint, int labelIndex, Brush brush)
    {
      if (ShowDataLabels)
      {
        object dependentData = dataPoint.YObject;
        DataLabel label;
        if (labelIndex < _dataLabels.Count)
        {
          label = _dataLabels[labelIndex];
          label.DataPoint = dataPoint;
          label.DependentData = dependentData;
        }
        else
        {
          label = new DataLabel(dataPoint, dependentData);
          label.SizeChanged += new SizeChangedEventHandler(Label_SizeChanged);
          _dataLabels.Add(label);
        }

        if (DataLabelStyle != null)
        {
          label.Style = DataLabelStyle;
        }
        label.Background = brush;

        PositionLabel(label, labelIndex);

        Canvas.Children.Add(label);
      }
    }

    private void Label_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      DataLabel label = sender as DataLabel;

      PositionLabel(label, -1);
    }

    private void PositionLabel(DataLabel label, int labelIndex)
    {
      CartesianDataPoint dataPoint = label.DataPoint as CartesianDataPoint;
      if (dataPoint != null)
      {
        Point currentPoint = dataPoint.LogicalPoint;
        Point normalPoint = ConvertLogicalToPhysicalPoint(currentPoint);

        double labelWidth = label.ActualWidth;
        double labelHeight = label.ActualHeight;

        double labelLeft = normalPoint.X + 10;
        double labelTop = normalPoint.Y + 10;

        /*if (normalPoint.Y < nextNormalPoint.Y)
        {
          labelTop = normalPoint.Y - 10 - labelHeight;
        }*/

        Point p = PerformLabelBoundsCorrection(labelLeft, labelTop, labelWidth, labelHeight);
        labelLeft = (int)p.X;
        labelTop = (int)p.Y;
        Canvas.SetLeft(label, labelLeft);
        Canvas.SetTop(label, labelTop);
        Canvas.SetZIndex(label, 200);

        if (Collides(p.X, p.Y, label, labelIndex))
        {
          label.Visibility = Visibility.Hidden;
        }
        else
        {
          label.Visibility = Visibility.Visible;
        }

        if (ShowDataLabelLines && label.Visibility == Visibility.Visible)
        {
          Line line = new Line() { X1 = normalPoint.X, Y1 = normalPoint.Y };
          line.Stroke = label.Background;
          line.StrokeThickness = 1; // TODO: provide a data label line style

          Point cornerPoint = GetClosestCornerPoint(normalPoint, labelLeft, labelTop, labelWidth, labelHeight);

          line.X2 = cornerPoint.X;
          line.Y2 = cornerPoint.Y;
          Canvas.SetZIndex(line, -1);
          Canvas.Children.Add(line);
        }
      }
    }

    private bool Collides(double x, double y, DataLabel label, int labelIndex)
    {
      if (label.ActualWidth > 0 && label.ActualHeight > 0)
      {
        Point centerPoint = new Point(x + label.ActualWidth / 2.0, y + label.ActualHeight / 2.0);
        int count = 0;
        foreach (DataLabel otherLabel in _dataLabels)
        {
          if (count == labelIndex)
          {
            break;
          }
          if (otherLabel != label && otherLabel.Visibility == Visibility.Visible && otherLabel.ActualWidth > 0 && otherLabel.ActualHeight > 0)
          {
            Point otherCenterPoint = new Point(Canvas.GetLeft(otherLabel) + otherLabel.ActualWidth / 2.0, Canvas.GetTop(otherLabel) + otherLabel.ActualHeight / 2.0);
            if (GeometryUtils.Distance(centerPoint, otherCenterPoint) < label.ActualWidth + otherLabel.ActualWidth)
            {
              return true;
            }
          }
          count++;
        }
      }
      return false;
    }

    private Point GetClosestCornerPoint(Point dataPoint, double left, double top, double width, double height)
    {
      Point result = new Point(left, top);
      double dist = GeometryUtils.Distance(dataPoint, result);

      Point point = new Point(left + width, top);
      double currentDist = GeometryUtils.Distance(dataPoint, point);
      if (currentDist < dist)
      {
        dist = currentDist;
        result = point;
      }

      point = new Point(left + width, top + height);
      currentDist = GeometryUtils.Distance(dataPoint, point);
      if (currentDist < dist)
      {
        dist = currentDist;
        result = point;
      }

      point = new Point(left, top + height);
      currentDist = GeometryUtils.Distance(dataPoint, point);
      if (currentDist < dist)
      {
        dist = currentDist;
        result = point;
      }

      return result;
    }
  }
}

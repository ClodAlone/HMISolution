using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Renders a heatmap on a <see cref="Chart"/>.
  /// </summary>
  public class HeatmapSeries : DataSeries
  {
    private Dictionary<double, Color> _brushCache;
    private Dictionary<float[], byte[]> _floatCache = new Dictionary<float[],byte[]>();
    private Dictionary<float[], byte[]> _secondFloatCache;
    private bool _isYReversed;
    private double _minBrushOffset;
    private double _maxBrushOffset;
    private double _resolution;

    private int _yMin;
    private int _yMax;
    private int _pixelWidth;
    private int _pixelHeight;
    private byte[] _pixels;

    static HeatmapSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(HeatmapSeries),
        new FrameworkPropertyMetadata(typeof(HeatmapSeries)));
    }

    #region MissingColor Property

    /// <summary>
    /// Gets or sets the <see cref="Color"/> used to display missing data.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MissingColorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Color MissingColor
    {
      get { return (Color)GetValue(MissingColorProperty); }
      set { SetValue(MissingColorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MissingColor"/> property.
    /// </summary>
    public static readonly DependencyProperty MissingColorProperty =
      DependencyProperty.Register("MissingColor", typeof(Color), typeof(HeatmapSeries),
      new FrameworkPropertyMetadata(OnMissingColorChanged));

    private static void OnMissingColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((HeatmapSeries)d).OnMissingColorChanged();
    }

    private void OnMissingColorChanged()
    {
      RequestRebuild();
    }

    #endregion // MissingColor Property

    /// <summary>
    /// Returns true.
    /// </summary>
    protected override bool SupportsDataSampling
    {
      get { return true; }
    }

    internal override void PrepareToPlotData()
    {
      _brushCache = new Dictionary<double, Color>();
      _secondFloatCache = new Dictionary<float[], byte[]>();
      _isYReversed = YAxis != null && YAxis.IsReversed;

      GradientBrush gradient = SeriesBrush as GradientBrush;
      if (gradient != null && gradient.GradientStops.Count > 0)
      {
        _minBrushOffset = gradient.GradientStops[0].Offset;
        _maxBrushOffset = gradient.GradientStops[gradient.GradientStops.Count - 1].Offset;
        _resolution = (_maxBrushOffset - _minBrushOffset) / 300.0;
      }

      _pixelWidth = MaximumIndex - MinimumIndex;
      _yMin = (int)Math.Floor(YAxis.ActualMinimumValue);
      _yMax = (int)Math.Ceiling(YAxis.ActualMaximumValue);
      _pixelHeight = _yMax - _yMin;
      _pixels = new byte[_pixelHeight * _pixelWidth * 4];
    }

    internal override void PlotDataPoint(int index)
    {
      Object o = ItemsSource[index];

      object xObject, yObject;
      GetPoint(o, index, out xObject, out yObject);

      double[] doubleData = yObject as double[];
      if (doubleData != null)
      {
        PlotDataPoint(doubleData, index);
      }
      else
      {
        float[] floatData = yObject as float[];
        PlotDataPoint(floatData, index);
      }
    }

    // TODO: how to simplify the code to support any type of array?
    private void PlotDataPoint(double[] data, int index)
    {
      Color missingColor = MissingColor;

      int columnIndex = index - MinimumIndex;
      if (XAxis != null && XAxis.IsReversed)
      {
        columnIndex = (MaximumIndex - MinimumIndex) - (index - MinimumIndex);
      }
      if (columnIndex >= 0 && columnIndex < _pixelWidth)
      {
        int dataCount = data == null ? (int)DependentMaximum : data.Length;
        int count = Math.Max(0, _yMax - dataCount);
        if (_isYReversed)
        {
          count = Math.Min(_pixelHeight, dataCount - _yMin) - 1;
        }
        for (int dataIndex = Math.Min(dataCount - 1, _yMax); dataIndex >= Math.Max(0, _yMin); dataIndex--)
        {
          int i = (count * _pixelWidth * 4) + (columnIndex * 4);
          if (i < _pixels.Length - 3 && i >= 0)
          {
            Color color = missingColor;
            if (data != null)
            {
              double d = data[dataIndex];
              if (!Double.IsNaN(d))
              {
                color = Interpolate(d);
              }
            }
            _pixels[i] = color.B;
            _pixels[i + 1] = color.G;
            _pixels[i + 2] = color.R;
            _pixels[i + 3] = 255;
            if (_isYReversed)
            {
              count--;
            }
            else
            {
              count++;
            }
          }
        }
      }
    }

    private void PlotDataPoint(float[] data, int index)
    {
      Color missingColor = MissingColor;

      int columnIndex = index - MinimumIndex;
      if (XAxis != null && XAxis.IsReversed)
      {
        columnIndex = (MaximumIndex - MinimumIndex) - (index - MinimumIndex);
      }
      if (columnIndex >= 0 && columnIndex < _pixelWidth)
      {
        int dataCount = data == null ? (int)DependentMaximum : data.Length;
        int count = Math.Max(0, _yMax - dataCount);
        if (_isYReversed)
        {
          count = Math.Min(_pixelHeight, dataCount - _yMin) - 1;
        }

        int expectedLength = (Math.Min(dataCount - 1, _yMax) - Math.Max(0, _yMin) + 1) * 4;
        byte[] previous;
        _floatCache.TryGetValue(data, out previous);
        if (previous != null && previous.Length == expectedLength)
        {
          int i = (count * _pixelWidth * 4) + (columnIndex * 4);
          for (int dataIndex = 0; dataIndex < previous.Length; dataIndex += 4)
          {
            if (i < _pixels.Length - 3 && i >= 0)
            {
              _pixels[i] = previous[dataIndex];
              _pixels[i + 1] = previous[dataIndex + 1];
              _pixels[i + 2] = previous[dataIndex + 2];
              _pixels[i + 3] = previous[dataIndex + 3];
              if (_isYReversed)
              {
                i -= _pixelWidth * 4;
              }
              else
              {
                i += _pixelWidth * 4;
              }
            }
          }
          _secondFloatCache[data] = previous;
        }
        else
        {
          previous = new byte[(Math.Min(dataCount - 1, _yMax) - Math.Max(0, _yMin) + 1) * 4];
          int previousIndex = 0;
          _secondFloatCache[data] = previous;
          for (int dataIndex = Math.Min(dataCount - 1, _yMax); dataIndex >= Math.Max(0, _yMin); dataIndex--)
          {
            int i = (count * _pixelWidth * 4) + (columnIndex * 4);
            if (i < _pixels.Length - 3 && i >= 0)
            {
              Color color = missingColor;
              if (data != null)
              {
                double d = data[dataIndex];
                if (!Double.IsNaN(d))
                {
                  color = Interpolate(d);
                }
              }
              _pixels[i] = color.B;
              _pixels[i + 1] = color.G;
              _pixels[i + 2] = color.R;
              _pixels[i + 3] = 255;
              previous[previousIndex] = _pixels[i];
              previous[previousIndex + 1] = _pixels[i + 1];
              previous[previousIndex + 2] = _pixels[i + 2];
              previous[previousIndex + 3] = _pixels[i + 3];
              previousIndex += 4;
              if (_isYReversed)
              {
                count--;
              }
              else
              {
                count++;
              }
            }
          }
        }
      }
    }

    internal override void FinishPlottingData()
    {
      _floatCache = _secondFloatCache;
      if (_pixelWidth > 0)
      {
        BitmapSource source = BitmapSource.Create(_pixelWidth, _pixelHeight, 96, 96, PixelFormats.Bgra32, null, _pixels, _pixelWidth * 4);
        Image image = new Image() { Source = source };
        image.Stretch = Stretch.Fill;
        image.Height = YAxis.ConvertLogicalToPhysicalSize(_pixelHeight);
        image.IsHitTestVisible = false;

        Object minDataPoint = ItemsSource[MinimumIndex];
        Object maxDataPoint = ItemsSource[MaximumIndex];
        Point logicalPoint = GetPoint(minDataPoint, MinimumIndex);
        double minPhysical = XAxis.ConvertLogicalToPhysical(logicalPoint.X);
        logicalPoint = GetPoint(maxDataPoint, MaximumIndex);
        double maxPhysical = XAxis.ConvertLogicalToPhysical(logicalPoint.X);
        Canvas.SetLeft(image, Math.Min(minPhysical, maxPhysical));
        image.Width = Math.Abs(maxPhysical - minPhysical);

        Canvas.SetTop(image, YAxis.ActualHeight - Math.Max(YAxis.ConvertLogicalToPhysical(_yMax), YAxis.ConvertLogicalToPhysical(_yMin)));

        RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
        Canvas.Children.Add(image);
      }
    }

    private Color Interpolate(double value)
    {
      Color result = new Color();
      double roundedValue = Round(value);

      if (_brushCache.ContainsKey(roundedValue))
      {
        return _brushCache[roundedValue];
      }
      else
      {
        GradientBrush brush = SeriesBrush as GradientBrush;
        if (brush != null)
        {
          GradientStop previousStop = null;
          foreach (GradientStop stop in brush.GradientStops)
          {
            if (stop.Offset == roundedValue)
            {
              result = stop.Color;
              break;
            }
            if (stop.Offset > roundedValue)
            {
              if (previousStop == null)
              {
                result = stop.Color;
                break;
              }
              else
              {
                Color previousColor = previousStop.Color;
                byte r1 = previousColor.R;
                byte g1 = previousColor.G;
                byte b1 = previousColor.B;
                double factor = (roundedValue - previousStop.Offset) / (stop.Offset - previousStop.Offset);
                Color c = stop.Color;
                byte r2 = c.R;
                byte g2 = c.G;
                byte b2 = c.B;
                double rDelta = (r2 - r1) * factor;
                double gDelta = (g2 - g1) * factor;
                double bDelta = (b2 - b1) * factor;
                byte r = (byte)Math.Max(0, Math.Min(255, r1 + rDelta));
                byte g = (byte)Math.Max(0, Math.Min(255, g1 + gDelta));
                byte b = (byte)Math.Max(0, Math.Min(255, b1 + bDelta));
                result = new Color() { R = r, G = g, B = b, A = 255 };
                break;
              }
            }
            previousStop = stop;
          }
          if (result == null && previousStop != null)
          {
            result = previousStop.Color;
          }
        }
        _brushCache[roundedValue] = result;
      }
      return result;
    }

    private double Round(double value)
    {
      return Math.Max(_minBrushOffset, Math.Min(_maxBrushOffset, ((int)((value - _minBrushOffset) / _resolution)) * _resolution));
    }
  }
}

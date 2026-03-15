using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements
{
  internal static class MeasurementUtils
  {
    internal static Canvas _panel = new Canvas();
    internal static TextBlock _textBlock = new TextBlock();

    static MeasurementUtils()
    {
      _panel.Children.Add(_textBlock);
    }

    internal static double GetTextWidth(string text)
    {
      _textBlock.Text = text;
      _panel.InvalidateMeasure();
      _panel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      return _textBlock.DesiredSize.Width;
    }

    internal static double GetTextHeight(string text)
    {
      _textBlock.Text = text;
      _panel.InvalidateMeasure();
      _panel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      return _textBlock.DesiredSize.Height;
    }
  }
}

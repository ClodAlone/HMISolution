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
using Mindscape.WpfElements.Charting;

namespace Mindscape.WpfElements.UnitTests
{
  /// <summary>
  /// This is a type of <see cref="IAxisValueConverter"/> used for testing.
  /// It simply makes the difference between the logical and physical values equal to 5.
  /// This converter is used for testing various parts of the charting classes to make sure they use
  /// the converter in appropriate situations.
  /// </summary>
  public class SimpleAxisValueConverter : IAxisValueConverter
  {
    public double GetAxisPlotPosition(object o)
    {
      double? value = NumericalUtils.ConvertToDouble(o);
      return value == null ? 0 : value.Value + 5;
    }

    public object GetDataObjectAt(double axisPosition)
    {
      return axisPosition - 5;
    }
  }
}

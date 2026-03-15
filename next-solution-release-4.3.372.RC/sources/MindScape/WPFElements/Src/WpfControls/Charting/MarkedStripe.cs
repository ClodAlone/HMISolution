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

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Holds information about rendering colored zones against a <see cref="ChartAxis"/>
  /// in a <see cref="MarkedStripeGrid"/>.
  /// </summary>
  public class MarkedStripe
  {
    private double _value;
    private Brush _background;

    /// <summary>
    /// Gets or sets the axis value that the <see cref="MarkedStripe"/> is below.
    /// </summary>
    public double Value
    {
      get { return _value; }
      set
      {
        if (value != _value)
        {
          _value = value;
          OnValueChanged();
        }
      }
    }

    internal event EventHandler ValueChanged;

    private void OnValueChanged()
    {
      EventHandler handler = ValueChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    /// <summary>
    /// Gets or sets the background brush of the <see cref="MarkedStripe"/>.
    /// </summary>
    public Brush Background
    {
      get { return _background; }
      set
      {
        if (value != _background)
        {
          _background = value;
          OnBackgroundChanged();
        }
      }
    }

    internal event EventHandler BackgroundChanged;

    private void OnBackgroundChanged()
    {
      EventHandler handler = BackgroundChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }
  }
}

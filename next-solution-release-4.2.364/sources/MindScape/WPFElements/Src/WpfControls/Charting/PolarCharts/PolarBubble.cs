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
  /// Represents a data point in a <see cref="PolarBubbleSeries"/>.
  /// </summary>
  public class PolarBubble : PolarDataPoint
  {
    private bool _isNegative;

    static PolarBubble()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarBubble),
        new FrameworkPropertyMetadata(typeof(PolarBubble)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarBubble"/> class.
    /// </summary>
    public PolarBubble()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarBubble"/> class.
    /// <param name="data">The data object that the <see cref="PolarBubble"/> plots.</param>
    /// </summary>
    internal PolarBubble(object data)
    {
      DataContext = data;
    }

    /// <summary>
    /// Gets whether or not this <see cref="PolarBubble"/> is plotting a negative value.
    /// </summary>
    public bool IsNegative
    {
      get { return _isNegative; }
      internal set { _isNegative = value; }
    }

    #region Size property

    /// <summary>
    /// Gets or sets the size of the bubble.
    /// This is a dependency property.
    /// </summary>
    public double Size
    {
      get { return (double)GetValue(SizeProperty); }
      set { SetValue(SizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Size"/> property.
    /// </summary>
    public static readonly DependencyProperty SizeProperty =
      DependencyProperty.Register("Size", typeof(double), typeof(PolarBubble), null);

    #endregion // Size property
  }
}

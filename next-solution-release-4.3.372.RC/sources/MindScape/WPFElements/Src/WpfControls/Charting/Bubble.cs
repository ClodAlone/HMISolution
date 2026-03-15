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
  /// Represents a data point in a <see cref="BubbleSeries"/>.
  /// </summary>
  public class Bubble : CartesianDataPoint
  {
    private bool _isNegative;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bubble"/> class.
    /// </summary>
    static Bubble()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Bubble),
        new FrameworkPropertyMetadata(typeof(Bubble)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bubble"/> class.
    /// </summary>
    public Bubble()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bubble"/> class.
    /// <param name="data">The data object that the <see cref="Bubble"/> plots.</param>
    /// </summary>
    internal Bubble(object data)
    {
      DataContext = data;
    }

    /// <summary>
    /// Gets whether or not this <see cref="Bubble"/> is plotting a negative value.
    /// </summary>
    public bool IsNegative
    {
      get { return _isNegative; }
      internal set { _isNegative = value; }
    }

    #region Size property

    /// <summary>
    /// Gets or sets the Size.
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
      DependencyProperty.Register("Size", typeof(double), typeof(Bubble),
      new PropertyMetadata(new PropertyChangedCallback(OnSizeChanged)));

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Bubble)d).OnSizeChanged();
    }

    private void OnSizeChanged()
    {
    }

    #endregion // Size property
  }
}

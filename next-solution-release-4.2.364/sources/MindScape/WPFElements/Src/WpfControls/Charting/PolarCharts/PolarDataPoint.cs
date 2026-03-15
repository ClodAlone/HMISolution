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
  /// Represents a single data point to be plotted on polar axes.
  /// </summary>
  public class PolarDataPoint : DataPoint
  {
    internal PolarDataPoint()
    {
    }

    #region ThetaObject property

    /// <summary>
    /// Gets or sets the object that specifies the theta value of the <see cref="PolarDataPoint"/>.
    /// This is a dependency property.
    /// </summary>
    public object ThetaObject
    {
      get { return GetValue(ThetaObjectProperty); }
      set { SetValue(ThetaObjectProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ThetaObject"/> property.
    /// </summary>
    public static readonly DependencyProperty ThetaObjectProperty =
      DependencyProperty.Register("ThetaObject", typeof(object), typeof(PolarDataPoint),
      new PropertyMetadata(new PropertyChangedCallback(OnThetaObjectChanged)));

    private static void OnThetaObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarDataPoint)d).OnThetaObjectChanged();
    }

    // I have made this internal for now, just in case we want to change the type of event handler to include other info such as the old value.
    internal event EventHandler ThetaObjectChanged;

    private void OnThetaObjectChanged()
    {
      EventHandler handler = ThetaObjectChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    #endregion // ThetaObject property

    #region RhoObject property

    /// <summary>
    /// Gets or sets the object that specifies the rho value of the <see cref="PolarDataPoint"/>.
    /// This is a dependency property.
    /// </summary>
    public object RhoObject
    {
      get { return GetValue(RhoObjectProperty); }
      set { SetValue(RhoObjectProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RhoObject"/> property.
    /// </summary>
    public static readonly DependencyProperty RhoObjectProperty =
      DependencyProperty.Register("RhoObject", typeof(object), typeof(PolarDataPoint),
      new PropertyMetadata(new PropertyChangedCallback(OnRhoObjectChanged)));

    private static void OnRhoObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarDataPoint)d).OnRhoObjectChanged();
    }

    // I have made this internal for now, just in case we want to change the type of event handler to include other info such as the old value.
    internal event EventHandler RhoObjectChanged;

    private void OnRhoObjectChanged()
    {
      EventHandler handler = RhoObjectChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    #endregion // RhoObject property

    /// <summary>
    /// Gets the logical theta,rho plotting position of the <see cref="PolarDataPoint"/>.
    /// </summary>
    public PolarPoint LogicalPoint { get; internal set; }
  }
}

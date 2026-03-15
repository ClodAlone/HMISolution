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
  /// Represents a data point in a <see cref="BarSeries"/>.
  /// </summary>
  public class Bar : CartesianDataPoint
  {
    static Bar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Bar),
        new FrameworkPropertyMetadata(typeof(Bar)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bar"/> class.
    /// <param name="data">The data plotted by the <see cref="Bar"/>.</param>
    /// <param name="orientation">The <see cref="Orientation"/> of the <see cref="Bar"/>.</param>
    /// </summary>
    internal Bar(object data, Orientation orientation)
    {
      DataContext = data;
      Orientation = orientation;
    }

    /// <summary>
    /// Gets whether or not the <see cref="Bar"/> is plotting a negative value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsNegativeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsNegative
    {
      get { return (bool)GetValue(IsNegativeProperty); }
      internal set { SetValue(IsNegativePropertyKey, value); }
    }

    private static readonly DependencyPropertyKey IsNegativePropertyKey =
        DependencyProperty.RegisterReadOnly("IsNegative", typeof(bool), typeof(Bar), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsNegative"/> property.
    /// </summary>
    public static readonly DependencyProperty IsNegativeProperty =
        IsNegativePropertyKey.DependencyProperty;



    /// <summary>
    /// Gets the <see cref="Orientation"/> of the <see cref="Bar"/>.
    /// </summary>
    public Orientation Orientation { get; private set; }

    #region Title property

    /// <summary>
    /// Gets or sets the optional title of the data point.
    /// This is a dependency property.
    /// </summary>
    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register("Title", typeof(string), typeof(Bar),
      new PropertyMetadata(new PropertyChangedCallback(OnTitleChanged)));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Bar)d).OnTitleChanged();
    }

    private void OnTitleChanged()
    {
    }

    #endregion // Title property
  }
}

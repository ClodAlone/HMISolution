using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control that displays a value and animates the visible value as it changes.
  /// </summary>
  public class AnimatedValueDisplay : RangeBase
  {
    private DoubleAnimation _animation;
    private Storyboard _storyBoard;

    static AnimatedValueDisplay()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedValueDisplay), new FrameworkPropertyMetadata(typeof(AnimatedValueDisplay)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedValueDisplay"/> class.
    /// </summary>
    public AnimatedValueDisplay()
    {
      _animation = new DoubleAnimation() { };
      Storyboard.SetTarget(_animation, this);
      Storyboard.SetTargetProperty(_animation, new PropertyPath(DisplayedValueProperty));
      _storyBoard = new Storyboard();
      _animation.Duration = new Duration(new TimeSpan(0, 0, 1));
      _storyBoard.Children.Add(_animation);
    }

    /// <summary>
    /// Called when the value changes.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    protected override void OnValueChanged(double oldValue, double newValue)
    {
      base.OnValueChanged(oldValue, newValue);

      _animation.To = Value;

      _storyBoard.Begin();
    }

    #region DisplayedValue property

    /// <summary>
    /// Gets the current value to be displayed. This value is animated whenever the Value property changes;
    /// a derived class or template can use it to display a smooth transition to the new value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DisplayedValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double DisplayedValue
    {
      get { return (double)GetValue(DisplayedValueProperty); }
    }

    /// <summary>
    /// Identifies the <see cref="DisplayedValue"/> property.
    /// </summary>
    public static readonly DependencyProperty DisplayedValueProperty =
      DependencyProperty.Register("DisplayedValue", typeof(double), typeof(AnimatedValueDisplay),
      new FrameworkPropertyMetadata(OnDisplayedValueChanged));

    private static void OnDisplayedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((AnimatedValueDisplay)d).OnDisplayedValueChanged();
    }

    /// <summary>
    /// Called when the displayed value changes during animation.  Derived classes can override this
    /// to update user interface elements in response to the animation.
    /// </summary>
    protected virtual void OnDisplayedValueChanged()
    {
    }

    #endregion // DisplayedValue property
  }
}

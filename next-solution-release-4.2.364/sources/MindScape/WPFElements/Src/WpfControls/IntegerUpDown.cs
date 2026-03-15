using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for editing an integer value with additional "up" and "down" commands.
  /// </summary>
  public class IntegerUpDown : IntegerRangeBase
  {
    static IntegerUpDown()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(typeof(IntegerUpDown)));

      MinimumProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(1));
      MaximumProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(Int32.MaxValue));
      ValueProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(1));
      SmallChangeProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(1));
      LargeChangeProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(10));
    }
  }

  /// <summary>
  /// Represents an element that has an integer value within a specific range. 
  /// </summary>
  public abstract class IntegerRangeBase : Control
  {
    #region Minimum Property

    /// <summary>
    /// Gets or sets the lowest possible value of the range control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int Minimum
    {
      get { return (int)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
      DependencyProperty.Register("Minimum", typeof(int), typeof(IntegerRangeBase),
      new FrameworkPropertyMetadata(OnMinimumChanged));

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((IntegerRangeBase)d).OnMinimumChanged();
    }

    private void OnMinimumChanged()
    {
      if (Minimum > Maximum)
      {
        Maximum = Minimum;
      }
    }

    #endregion // Minimum Property

    #region Maximum Property

    /// <summary>
    /// Gets or sets the highest possible value of the range control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaximumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int Maximum
    {
      get { return (int)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
      DependencyProperty.Register("Maximum", typeof(int), typeof(IntegerRangeBase),
      new FrameworkPropertyMetadata(OnMaximumChanged));

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((IntegerRangeBase)d).OnMaximumChanged();
    }

    private void OnMaximumChanged()
    {
      if (Maximum < Minimum)
      {
        Minimum = Maximum;
      }
    }

    #endregion // Maximum Property

    #region Value Property

    /// <summary>
    /// Gets or sets the current magnitude of the range control
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int Value
    {
      get { return (int)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register("Value", typeof(int), typeof(IntegerRangeBase),
      new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((IntegerRangeBase)d).OnValueChanged();
    }

    private void OnValueChanged()
    {
      if (Value < Minimum)
      {
        Value = Minimum;
      }
      else if(Value > Maximum)
      {
        Value = Maximum;
      }
    }

    #endregion // Value Property

    #region SmallChange Property

    /// <summary>
    /// Gets or sets a value to be added to or subtracted from the Value of an IntegerRangeBase control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SmallChangeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int SmallChange
    {
      get { return (int)GetValue(SmallChangeProperty); }
      set { SetValue(SmallChangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SmallChange"/> property.
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty =
      DependencyProperty.Register("SmallChange", typeof(int), typeof(IntegerRangeBase));

    #endregion // SmallChange Property

    #region LargeChange Property

    /// <summary>
    /// Gets or sets a value to be added to or subtracted from the Value of an IntegerRangeBase control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LargeChangeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int LargeChange
    {
      get { return (int)GetValue(LargeChangeProperty); }
      set { SetValue(LargeChangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LargeChange"/> property.
    /// </summary>
    public static readonly DependencyProperty LargeChangeProperty =
      DependencyProperty.Register("LargeChange", typeof(int), typeof(IntegerRangeBase));

    #endregion // LargeChange Property
  }
}

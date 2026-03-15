using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using Infralution.Licensing;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for modifying a numeric value using up/down buttons.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class Spin : Control
  {
    static Spin()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Spin), 
        new FrameworkPropertyMetadata(typeof(Spin)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Spin"/> class.
    /// </summary>
    public Spin()
    {
      BindCommands();
    }

    private void BindCommands()
    {
      CommandBindings.Add(new CommandBinding(SpinCommands.Increase, Increase_Executed));
      CommandBindings.Add(new CommandBinding(SpinCommands.Decrease, Decrease_Executed));
    }

    private void Increase_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (Value + Change <= Maximum)
      {
        Value += Change;
      }
    }

    private void Decrease_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (Value - Change >= Minimum)
      {
        Value -= Change;
      }
    }

    /// <summary>
    /// Gets or sets the amount by which the value is changed when the user
    /// spins the control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ChangeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public decimal Change
    {
      get { return (decimal)GetValue(ChangeProperty); }
      set { SetValue(ChangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Change"/> property.
    /// </summary>
    public static readonly DependencyProperty ChangeProperty =
        DependencyProperty.Register("Change", typeof(decimal), typeof(Spin), 
        new FrameworkPropertyMetadata(1m));

    /// <summary>
    /// Gets or sets the value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>BindsTwoWayByDefault</td></tr>
    /// </table>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "By analogy with RangeBase")]
    public decimal Value
    {
      get { return (decimal)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(decimal), typeof(Spin), 
        new FrameworkPropertyMetadata(
          0m, 
          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


    /// <summary>
    /// Gets or sets the minimum value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public decimal Minimum
    {
      get { return (decimal)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register("Minimum", typeof(decimal), typeof(Spin), 
        new FrameworkPropertyMetadata(Decimal.MinValue));


    /// <summary>
    /// Gets or sets the maximum value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaximumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public decimal Maximum
    {
      get { return (decimal)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register("Maximum", typeof(decimal), typeof(Spin), 
        new FrameworkPropertyMetadata(Decimal.MaxValue));



    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Style"/> for spin
    /// buttons.
    /// </summary>
    public static object SpinButtonStyleKey
    {
      get { return new ComponentResourceKey(typeof(Spin), "SpinButtonStyle"); }
    }
  }
}

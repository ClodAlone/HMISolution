using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using Infralution.Licensing;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Associates a control with a <see cref="Spin"/> control.
  /// </summary>
  [TemplatePart(Name = SpinPartName, Type = typeof(Spin))]
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class SpinDecorator : ContentControl
  {
    private const string SpinPartName = "PART_Spin";

    static SpinDecorator()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SpinDecorator),
        new FrameworkPropertyMetadata(typeof(SpinDecorator)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpinDecorator"/> class.
    /// </summary>
    public SpinDecorator()
    {
      PreviewKeyDown += new KeyEventHandler(SpinDecorator_PreviewKeyDown);
    }

    private void SpinDecorator_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      switch (e.Key)
      {
        case Key.Up: Increase(); e.Handled = true; break;
        case Key.Down: Decrease(); e.Handled = true; break;
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
      Spin.ChangeProperty.AddOwner(typeof(SpinDecorator));

    /// <summary>
    /// Gets or sets the name of the property on the child control which will be
    /// modified when the user spins the control.  The default is "Value".
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ValuePropertyProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string ValueProperty
    {
      get { return (string)GetValue(ValuePropertyProperty); }
      set { SetValue(ValuePropertyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ValueProperty"/> property.
    /// </summary>
    public static readonly DependencyProperty ValuePropertyProperty =
        DependencyProperty.Register("ValueProperty", typeof(string), typeof(SpinDecorator),
        new FrameworkPropertyMetadata("Value"));


    /// <summary>
    /// Gets or sets the name of the property on the child control which controls
    /// the minimum value that can be entered directly.  The default is "Minimum".
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumPropertyProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string MinimumProperty
    {
      get { return (string)GetValue(MinimumPropertyProperty); }
      set { SetValue(MinimumPropertyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinimumProperty"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumPropertyProperty =
        DependencyProperty.Register("MinimumProperty", typeof(string), typeof(SpinDecorator),
        new FrameworkPropertyMetadata("Minimum"));


    /// <summary>
    /// Gets or sets the name of the property on the child control which controls
    /// the maximum value that can be entered directly.  The default is "Maximum".
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaximumPropertyProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string MaximumProperty
    {
      get { return (string)GetValue(MaximumPropertyProperty); }
      set { SetValue(MaximumPropertyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaximumProperty"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumPropertyProperty =
        DependencyProperty.Register("MaximumProperty", typeof(string), typeof(SpinDecorator),
        new FrameworkPropertyMetadata("Maximum"));

    /// <summary>
    /// Gets or sets whether the spin UI should be shown.  If false, the value
    /// can still be spun using the cursor keys.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowSpinUIProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowSpinUI
    {
      get { return (bool)GetValue(ShowSpinUIProperty); }
      set { SetValue(ShowSpinUIProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowSpinUI"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowSpinUIProperty =
        DependencyProperty.Register("ShowSpinUI", typeof(bool), typeof(SpinDecorator),
        new FrameworkPropertyMetadata(true));

    private FrameworkElement _spin;

    /// <summary>
    /// Called by the framework when a template is applied to the control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _spin = GetTemplateChild("PART_Spin") as FrameworkElement;
      if (_spin != null)
      {
        BindSpinPropertyToChildControlProperty(Spin.ValueProperty, ValueProperty);
        BindSpinPropertyToChildControlProperty(Spin.MinimumProperty, MinimumProperty);
        BindSpinPropertyToChildControlProperty(Spin.MaximumProperty, MaximumProperty);
      }
    }

    private void BindSpinPropertyToChildControlProperty(DependencyProperty propertyToBind, string childPropertyName)
    {
      Debug.Assert(_spin != null);

      Binding binding = new Binding(childPropertyName);
      binding.Source = Content;
      _spin.SetBinding(propertyToBind, binding);
    }

    private void ExecuteSpinCommand(RoutedCommand command)
    {
      if (_spin != null)
      {
        command.Execute(null, _spin);
      }
    }

    private void Increase()
    {
      ExecuteSpinCommand(SpinCommands.Increase);
    }

    private void Decrease()
    {
      ExecuteSpinCommand(SpinCommands.Decrease);
    }
  }
}

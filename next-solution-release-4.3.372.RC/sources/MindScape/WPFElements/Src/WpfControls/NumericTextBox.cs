using System;
using System.ComponentModel;
using System.Windows;
using Infralution.Licensing;
using System.Diagnostics;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for entering numeric values.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class NumericTextBox : NumericTextBoxBase<decimal>
  {
    //private const string TextBoxPartName = "PART_TextBox";

    private readonly NumericTextBoxModel _model = new NumericTextBoxModel();

    static NumericTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBox), 
        new FrameworkPropertyMetadata(typeof(NumericTextBox)));
      MinimumProperty.OverrideMetadata(typeof(NumericTextBox),
        new FrameworkPropertyMetadata(Decimal.MinValue));
      MaximumProperty.OverrideMetadata(typeof(NumericTextBox),
        new FrameworkPropertyMetadata(Decimal.MaxValue));
    }

    internal override NumericTextBoxBaseModel<decimal> NumericModel
    {
      get { return _model; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericTextBox"/> class.
    /// </summary>
    public NumericTextBox()
    {
      GotFocus += new RoutedEventHandler(NumericTextBox_GotFocus);
      LostFocus += new RoutedEventHandler(NumericTextBox_LostFocus);
    }

    private void NumericTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      if (FocusChangedBehavior == NumericTextBoxFocusChangedBehavior.Round)
      {
        _model.RefreshTextFromValueIgnoreDecimalPlaces();
        UpdateDisplay();
      }
    }

    private void NumericTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      if (FocusChangedBehavior == NumericTextBoxFocusChangedBehavior.Round)
      {
        _model.RefreshTextFromValue();
        UpdateDisplay();
      }
    }

    /// <summary>
    /// Gets or sets the number of decimal places to display.  Set to -1
    /// to display the default number of decimal places for the culture.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DecimalPlacesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int DecimalPlaces
    {
      get { return (int)GetValue(DecimalPlacesProperty); }
      set { SetValue(DecimalPlacesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DecimalPlaces"/> property.
    /// </summary>
    public static readonly DependencyProperty DecimalPlacesProperty =
      DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumericTextBox),
      new FrameworkPropertyMetadata(-1, OnDecimalPlacesChanged));

    private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumericTextBox)d).OnDecimalPlacesChanged();
    }

    private void OnDecimalPlacesChanged()
    {
      _model.DecimalPlaces = DecimalPlaces;
      UpdateIfInitialised();
    }

    /// <summary>
    /// Gets or sets whether the user is prevented from typing beyond the permitted
    /// number of decimal places.  Ignored if DecimalPlaces is -1.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EnforceDecimalPlacesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool EnforceDecimalPlaces
    {
      get { return (bool)GetValue(EnforceDecimalPlacesProperty); }
      set { SetValue(EnforceDecimalPlacesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EnforceDecimalPlaces"/> property.
    /// </summary>
    public static readonly DependencyProperty EnforceDecimalPlacesProperty =
      DependencyProperty.Register("EnforceDecimalPlaces", typeof(bool), typeof(NumericTextBox),
      new FrameworkPropertyMetadata(true, OnEnforceDecimalPlacesChanged));

    private static void OnEnforceDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumericTextBox)d).OnEnforceDecimalPlacesChanged();
    }

    private void OnEnforceDecimalPlacesChanged()
    {
      _model.EnforceDecimalPlaces = EnforceDecimalPlaces;
    }

    #region FocusChangedBehavior Property

    /// <summary>
    /// Gets or sets the beahvior of the <see cref="NumericTextBox"/> when the focus changes.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FocusChangedBehaviorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public NumericTextBoxFocusChangedBehavior FocusChangedBehavior
    {
      get { return (NumericTextBoxFocusChangedBehavior)GetValue(FocusChangedBehaviorProperty); }
      set { SetValue(FocusChangedBehaviorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FocusChangedBehavior"/> property.
    /// </summary>
    public static readonly DependencyProperty FocusChangedBehaviorProperty =
      DependencyProperty.Register("FocusChangedBehavior", typeof(NumericTextBoxFocusChangedBehavior), typeof(NumericTextBox),
      new FrameworkPropertyMetadata(NumericTextBoxFocusChangedBehavior.None, OnFocusChangedBehaviorChanged));

    private static void OnFocusChangedBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((NumericTextBox)d).OnFocusChangedBehaviorChanged();
    }

    private void OnFocusChangedBehaviorChanged()
    {
    }

    #endregion // FocusChangedBehavior Property
  }

  /// <summary>
  /// A control for entering numeric values of type double.
  /// </summary>
  public class DoubleTextBox : NumericTextBoxBase<double>
  {
    //private const string TextBoxPartName = "PART_TextBox";

    private readonly DoubleTextBoxModel _model = new DoubleTextBoxModel();

    static DoubleTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleTextBox),
        new FrameworkPropertyMetadata(typeof(DoubleTextBox)));
      MinimumProperty.OverrideMetadata(typeof(DoubleTextBox),
        new FrameworkPropertyMetadata(Double.MinValue));
      MaximumProperty.OverrideMetadata(typeof(DoubleTextBox),
        new FrameworkPropertyMetadata(Double.MaxValue));
    }

    internal override NumericTextBoxBaseModel<double> NumericModel
    {
      get { return _model; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleTextBox"/> class.
    /// </summary>
    public DoubleTextBox()
    {
      GotFocus += new RoutedEventHandler(DoubleTextBox_GotFocus);
      LostFocus += new RoutedEventHandler(DoubleTextBox_LostFocus);
    }

    private void DoubleTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      if (FocusChangedBehavior == NumericTextBoxFocusChangedBehavior.Round)
      {
        _model.RefreshTextFromValueIgnoreDecimalPlaces();
        UpdateDisplay();
      }
    }

    private void DoubleTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      if (FocusChangedBehavior == NumericTextBoxFocusChangedBehavior.Round)
      {
        _model.RefreshTextFromValue();
        UpdateDisplay();
      }
    }

    /// <summary>
    /// Gets or sets the number of decimal places to display.  Set to -1
    /// to display the default number of decimal places for the culture.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DecimalPlacesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int DecimalPlaces
    {
      get { return (int)GetValue(DecimalPlacesProperty); }
      set { SetValue(DecimalPlacesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DecimalPlaces"/> property.
    /// </summary>
    public static readonly DependencyProperty DecimalPlacesProperty =
      DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(DoubleTextBox),
      new FrameworkPropertyMetadata(-1, OnDecimalPlacesChanged));

    private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DoubleTextBox)d).OnDecimalPlacesChanged();
    }

    private void OnDecimalPlacesChanged()
    {
      _model.DecimalPlaces = DecimalPlaces;
      UpdateIfInitialised();
    }

    /// <summary>
    /// Gets or sets whether the user is prevented from typing beyond the permitted
    /// number of decimal places.  Ignored if DecimalPlaces is -1.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EnforceDecimalPlacesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool EnforceDecimalPlaces
    {
      get { return (bool)GetValue(EnforceDecimalPlacesProperty); }
      set { SetValue(EnforceDecimalPlacesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EnforceDecimalPlaces"/> property.
    /// </summary>
    public static readonly DependencyProperty EnforceDecimalPlacesProperty =
      DependencyProperty.Register("EnforceDecimalPlaces", typeof(bool), typeof(DoubleTextBox),
      new FrameworkPropertyMetadata(true, OnEnforceDecimalPlacesChanged));

    private static void OnEnforceDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DoubleTextBox)d).OnEnforceDecimalPlacesChanged();
    }

    private void OnEnforceDecimalPlacesChanged()
    {
      _model.EnforceDecimalPlaces = EnforceDecimalPlaces;
    }

    #region FocusChangedBehavior Property

    /// <summary>
    /// Gets or sets the beahvior of the <see cref="DoubleTextBox"/> when the focus changes.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FocusChangedBehaviorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public NumericTextBoxFocusChangedBehavior FocusChangedBehavior
    {
      get { return (NumericTextBoxFocusChangedBehavior)GetValue(FocusChangedBehaviorProperty); }
      set { SetValue(FocusChangedBehaviorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FocusChangedBehavior"/> property.
    /// </summary>
    public static readonly DependencyProperty FocusChangedBehaviorProperty =
      DependencyProperty.Register("FocusChangedBehavior", typeof(NumericTextBoxFocusChangedBehavior), typeof(DoubleTextBox),
      new FrameworkPropertyMetadata(NumericTextBoxFocusChangedBehavior.None, OnFocusChangedBehaviorChanged));

    private static void OnFocusChangedBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DoubleTextBox)d).OnFocusChangedBehaviorChanged();
    }

    private void OnFocusChangedBehaviorChanged()
    {
    }

    #endregion // FocusChangedBehavior Property
  }

  /// <summary>
  /// A control for entering integer values.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class IntegerTextBox : NumericTextBoxBase<int>
  {
    //private const string TextBoxPartName = "PART_TextBox";

    private readonly IntegerTextBoxModel _model = new IntegerTextBoxModel();

    static IntegerTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerTextBox),
        new FrameworkPropertyMetadata(typeof(IntegerTextBox)));
      MinimumProperty.OverrideMetadata(typeof(IntegerTextBox),
        new FrameworkPropertyMetadata(Int32.MinValue));
      MaximumProperty.OverrideMetadata(typeof(IntegerTextBox),
        new FrameworkPropertyMetadata(Int32.MaxValue));
    }

    internal override NumericTextBoxBaseModel<int> NumericModel
    {
      get { return _model; }
    }

    /// <summary>
    /// Gets or sets the precision (the minimum number of digits that should be
    /// displayed).  The control will pad the text with leading zeroes if required.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <para>If Precision is specified (non-zero), then ShowSeparators is ignored.</para>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PrecisionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int Precision
    {
      get { return (int)GetValue(PrecisionProperty); }
      set { SetValue(PrecisionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Precision"/> property.
    /// </summary>
    public static readonly DependencyProperty PrecisionProperty =
        DependencyProperty.Register("Precision", typeof(int), typeof(IntegerTextBox),
        new FrameworkPropertyMetadata(OnPrecisionChanged));

    private static void OnPrecisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((IntegerTextBox)d).OnPrecisionChanged();
    }

    private void OnPrecisionChanged()
    {
      _model.Precision = Precision;
      UpdateIfInitialised();
    }

    /// <summary>
    /// Gets or sets whether the user is prevented from typing beyond the permitted
    /// number of digits (precision).  Ignored if Precision is 0.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EnforcePrecisionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool EnforcePrecision
    {
      get { return (bool)GetValue(EnforcePrecisionProperty); }
      set { SetValue(EnforcePrecisionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EnforcePrecision"/> property.
    /// </summary>
    public static readonly DependencyProperty EnforcePrecisionProperty =
      DependencyProperty.Register("EnforcePrecision", typeof(bool), typeof(IntegerTextBox),
      new FrameworkPropertyMetadata(true, OnEnforcePrecisionChanged));

    private static void OnEnforcePrecisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((IntegerTextBox)d).OnEnforcePrecisionChanged();
    }

    private void OnEnforcePrecisionChanged()
    {
      _model.EnforcePrecision = EnforcePrecision;
    }
  }

  /// <summary>
  /// Specifies the behavior of a <see cref="NumericTextBox"/> when it loses or gains focus.
  /// </summary>
  public enum NumericTextBoxFocusChangedBehavior
  {
    /// <summary>
    /// The displayed text value does not change.
    /// </summary>
    None,

    /// <summary>
    /// When the <see cref="NumericTextBox"/> loses focus, the displayed text value will be rounded based on the DecimalPlaces property.
    /// When the <see cref="NumericTextBox"/> gets focus, any previous user input will be restored, allowing them to continue editing the value.
    /// </summary>
    Round
  }
}
